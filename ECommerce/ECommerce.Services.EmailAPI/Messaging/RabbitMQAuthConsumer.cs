using ECommerce.Services.EmailAPI.Service.IService;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ECommerce.Services.EmailAPI.Messaging
{
    public class RabbitMQAuthConsumer : BackgroundService
    {
        private readonly string _queueName;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RabbitMQAuthConsumer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        private IConnection? _connection;
        private IChannel? _channel;

        // used to wake up the loop when the connection/channel dies
        private TaskCompletionSource<bool>? _connectionClosedTcs;

        public RabbitMQAuthConsumer(IConfiguration configuration,
        IServiceScopeFactory scopeFactory,
        ILogger<RabbitMQAuthConsumer> logger)
        {
            _configuration = configuration;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _queueName = _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue")!;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _connectionClosedTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

                    await ConnectAndStartConsumingAsync(stoppingToken);

                    // Wait until either:
                    // - app is stopping
                    // - connection/channel signals closed
                    await Task.WhenAny(
                        Task.Delay(Timeout.Infinite, stoppingToken),
                        _connectionClosedTcs.Task
                    );
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // normal shutdown
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "RabbitMQ consumer crashed. Retrying in 5 seconds...");
                    await SafeCloseAsync();

                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { }
                }
                finally
                {
                    await SafeCloseAsync();
                }
            }
        }

        private async Task ConnectAndStartConsumingAsync(CancellationToken ct)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration.GetValue<string>("RabbitMQ:HostName") ?? "localhost",
                UserName = _configuration.GetValue<string>("RabbitMQ:UserName") ?? "guest",
                Password = _configuration.GetValue<string>("RabbitMQ:Password") ?? "guest",
                VirtualHost = _configuration.GetValue<string>("RabbitMQ:VirtualHost") ?? "/",
                Port = _configuration.GetValue<int?>("RabbitMQ:Port") ?? 5672,

                AutomaticRecoveryEnabled = false // because we have our own reconnect loop
            };

            _connection = await factory.CreateConnectionAsync(ct);

            // Don’t await heavy work here. Just signal the loop.
            _connection.ConnectionShutdownAsync += (_, ea) =>
            {
                _logger.LogWarning("RabbitMQ connection shutdown: {ReplyText}", ea.ReplyText);
                _connectionClosedTcs?.TrySetResult(true);
                return Task.CompletedTask;
            };

            _channel = await _connection.CreateChannelAsync();

            _channel.ChannelShutdownAsync += (_, ea) =>
            {
                _logger.LogWarning("RabbitMQ channel shutdown: {ReplyText}", ea.ReplyText);
                _connectionClosedTcs?.TrySetResult(true);
                return Task.CompletedTask;
            };

            // IMPORTANT: these flags must match the existing queue everywhere
            await _channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: ct);

            // Control in-flight messages per consumer
            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 10, global: false, cancellationToken: ct);

            // Capture the channel locally so a reconnect won't swap the instance out from under the handler.
            var channel = _channel;

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += (sender, evt) => OnMessageAsync(channel, evt, ct);

            await channel.BasicConsumeAsync(
                queue: _queueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: ct);

            _logger.LogInformation("RabbitMQ consumer started. Queue={Queue}", _queueName);
        }

        private async Task OnMessageAsync(IChannel channel, BasicDeliverEventArgs evt, CancellationToken ct)
        {
            try
            {
                var messageJson = Encoding.UTF8.GetString(evt.Body.ToArray());
                var userEmail = JsonConvert.DeserializeObject<string>(messageJson);

                // Resolve scoped dependencies per message
                using var scope = _scopeFactory.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                await emailService.RegisterUserEmailAndLog(userEmail!);

                await channel.BasicAckAsync(evt.DeliveryTag, multiple: false, cancellationToken: ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                // app is shutting down - don't ack, let it be re-delivered
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message. DeliveryTag={Tag}", evt.DeliveryTag);

                // Prefer DLX and poison-message handling (requeue false).
                // If you need transient retries, implement a retry policy + counter instead of infinite requeue loops.
                try
                {
                    await channel.BasicNackAsync(evt.DeliveryTag, multiple: false, requeue: false, cancellationToken: ct);
                }
                catch (Exception nackEx)
                {
                    _logger.LogWarning(nackEx, "Failed to Nack message. DeliveryTag={Tag}", evt.DeliveryTag);
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _connectionClosedTcs?.TrySetResult(true);
            await SafeCloseAsync();
            await base.StopAsync(cancellationToken);
        }

        private async Task SafeCloseAsync()
        {
            try
            {
                if (_channel is not null)
                {
                    try { await _channel.CloseAsync(); } catch { }
                    await _channel.DisposeAsync();
                    _channel = null;
                }
            }
            catch { /* ignore */ }

            try
            {
                if (_connection is not null)
                {
                    try { await _connection.CloseAsync(); } catch { }
                    await _connection.DisposeAsync();
                    _connection = null;
                }
            }
            catch { /* ignore */ }
        }
    }
}

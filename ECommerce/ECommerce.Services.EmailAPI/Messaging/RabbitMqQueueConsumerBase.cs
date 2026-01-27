using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ECommerce.Services.EmailAPI.Messaging
{
    public abstract class RabbitMqQueueConsumerBase<TMessage> : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger _logger;

        private IConnection? _connection;
        private IChannel? _channel;
        private TaskCompletionSource<bool>? _closedTcs;

        protected RabbitMqQueueConsumerBase(
            IConfiguration configuration,
            IServiceScopeFactory scopeFactory,
            ILogger logger)
        {
            _configuration = configuration;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected abstract string QueueConfigKey { get; }          // e.g. "TopicAndQueueNames:RegisterUserQueue"
        protected virtual bool Durable => true;
        protected virtual ushort PrefetchCount => 10;

        protected abstract TMessage Deserialize(ReadOnlyMemory<byte> body);
        protected abstract Task HandleAsync(IServiceProvider sp, TMessage message, CancellationToken ct);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var queueName = _configuration.GetValue<string>(QueueConfigKey)!;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _closedTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

                    await ConnectAndConsumeAsync(queueName, stoppingToken);

                    await Task.WhenAny(
                        Task.Delay(Timeout.Infinite, stoppingToken),
                        _closedTcs.Task
                    );
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // normal shutdown
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "RabbitMQ consumer for {Queue} crashed. Retrying in 5 seconds...", queueName);
                    await SafeCloseAsync();
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                finally
                {
                    await SafeCloseAsync();
                }
            }
        }

        private async Task ConnectAndConsumeAsync(string queueName, CancellationToken ct)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration.GetValue<string>("RabbitMQ:HostName") ?? "localhost",
                UserName = _configuration.GetValue<string>("RabbitMQ:UserName") ?? "guest",
                Password = _configuration.GetValue<string>("RabbitMQ:Password") ?? "guest",
                VirtualHost = _configuration.GetValue<string>("RabbitMQ:VirtualHost") ?? "/",
                Port = _configuration.GetValue<int?>("RabbitMQ:Port") ?? 5672,
                AutomaticRecoveryEnabled = false
            };

            _connection = await factory.CreateConnectionAsync(ct);
            _connection.ConnectionShutdownAsync += (_, ea) =>
            {
                _logger.LogWarning("RabbitMQ connection shutdown ({Queue}): {ReplyText}", queueName, ea.ReplyText);
                _closedTcs?.TrySetResult(true);
                return Task.CompletedTask;
            };

            _channel = await _connection.CreateChannelAsync();
            _channel.ChannelShutdownAsync += (_, ea) =>
            {
                _logger.LogWarning("RabbitMQ channel shutdown ({Queue}): {ReplyText}", queueName, ea.ReplyText);
                _closedTcs?.TrySetResult(true);
                return Task.CompletedTask;
            };

            await _channel.QueueDeclareAsync(queueName, durable: Durable, exclusive: false, autoDelete: false, arguments: null, cancellationToken: ct);
            await _channel.BasicQosAsync(0, PrefetchCount, false, ct);

            var channel = _channel; // capture

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (_, evt) =>
            {
                using var scope = _scopeFactory.CreateScope();
                try
                {
                    var msg = Deserialize(evt.Body);
                    await HandleAsync(scope.ServiceProvider, msg, ct);
                    await channel.BasicAckAsync(evt.DeliveryTag, false, ct);
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    // shutdown
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message from {Queue}. DeliveryTag={Tag}", queueName, evt.DeliveryTag);
                    await channel.BasicNackAsync(evt.DeliveryTag, false, requeue: false, ct);
                }
            };

            await channel.BasicConsumeAsync(queueName, autoAck: false, consumer: consumer, cancellationToken: ct);

            _logger.LogInformation("RabbitMQ consumer started. Queue={Queue}", queueName);
        }

        private async Task SafeCloseAsync()
        {
            if (_channel is not null)
            {
                try { await _channel.CloseAsync(); } catch { }
                try { await _channel.DisposeAsync(); } catch { }
                _channel = null;
            }

            if (_connection is not null)
            {
                try { await _connection.CloseAsync(); } catch { }
                try { await _connection.DisposeAsync(); } catch { }
                _connection = null;
            }
        }
    }
}

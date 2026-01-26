using ECommerce.Services.EmailAPI.Service.IService;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ECommerce.Services.EmailAPI.Messaging
{
    public class RabbitMQAuthConsumer : BackgroundService
    {
        private readonly string registerUserQueue;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ILogger<RabbitMQAuthConsumer> _logger;
        private IConnection _connection;
        private IChannel _channel;

        public RabbitMQAuthConsumer(IConfiguration configuration, IEmailService emailService, ILogger<RabbitMQAuthConsumer> logger)
        {
            _configuration = configuration;
            _emailService = emailService;
            _logger = logger;
            registerUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue")!;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ConnectAndStartConsumingAsync(stoppingToken);

                    // block here until cancellation or a connection/channel shutdown triggers an exception
                    await Task.Delay(Timeout.Infinite, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // normal shutdown
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "RabbitMQ consumer crashed. Will retry in 5 seconds...");
                    await SafeCloseAsync();
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await SafeCloseAsync();
            await base.StopAsync(cancellationToken);
        }

        private async Task ConnectAndStartConsumingAsync(CancellationToken ct)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration.GetValue<string>("RabbitMQ:HostName") ?? "localhost",
                UserName = _configuration.GetValue<string>("RabbitMQ:UserName") ?? "guest",
                Password = _configuration.GetValue<string>("RabbitMQ:Password") ?? "guest",
                AutomaticRecoveryEnabled = false // if you implement your own retry loop


                //HostName = _configuration.GetValue<string>("RabbitMQ:HostName"),
                //UserName = _configuration.GetValue<string>("RabbitMQ:UserName"),
                //Password = _configuration.GetValue<string>("RabbitMQ:Password")
            };

            _connection = await factory.CreateConnectionAsync(ct);
            _connection.ConnectionShutdownAsync += async (_, ea) =>
            {
                _logger.LogWarning("RabbitMQ connection shutdown: {ReplyText}", ea.ReplyText);
                await SafeCloseAsync();
            };

            _channel = await _connection.CreateChannelAsync();
            _channel.ChannelShutdownAsync += async (_, ea) =>
            {
                _logger.LogWarning("RabbitMQ channel shutdown: {ReplyText}", ea.ReplyText);
                await SafeCloseAsync();
            };

            await _channel.QueueDeclareAsync(registerUserQueue, durable: false, exclusive: false, autoDelete: false, arguments: null, cancellationToken: ct);

            // Optional but recommended: prefetch so you don't get flooded
            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 10, global: false, cancellationToken: ct);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += OnMessageAsync;

            // Start consuming ONCE
            await _channel.BasicConsumeAsync(queue: registerUserQueue, autoAck: false, consumer: consumer, cancellationToken: ct);

            _logger.LogInformation("RabbitMQ consumer started. Queue={Queue}", registerUserQueue);
        }

        private async Task OnMessageAsync(object sender, BasicDeliverEventArgs evt)
        {
            if (_channel is null) return;

            try
            {
                var message = Encoding.UTF8.GetString(evt.Body.ToArray());
                var registerUserMessage = JsonConvert.DeserializeObject<string>(message);

                await HandleMessage(registerUserMessage!);

                await _channel.BasicAckAsync(evt.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing RabbitMQ message. DeliveryTag={Tag}", evt.DeliveryTag);

                // Decide requeue policy:
                // - requeue:true can cause infinite poison-message loops
                // - requeue:false works best with DLX/dead-letter queue configured on the queue
                await _channel.BasicNackAsync(evt.DeliveryTag, multiple: false, requeue: false);
            }
        }

        //private async Task EstablishConnection()
        //{
        //    var factory = new ConnectionFactory()
        //    {
        //        HostName = "localhost",
        //        UserName = "guest",
        //        Password = "guest"

        //        //HostName = _configuration.GetValue<string>("RabbitMQ:HostName"),
        //        //UserName = _configuration.GetValue<string>("RabbitMQ:UserName"),
        //        //Password = _configuration.GetValue<string>("RabbitMQ:Password")
        //    };

        //    _connection = await factory.CreateConnectionAsync();
        //    _channel = await _connection.CreateChannelAsync();
        //    await _channel.QueueDeclareAsync(registerUserQueue, false, false, false, null);
        //}

        private async Task HandleMessage(string registerUserMessage)
            => await _emailService.RegisterUserEmailAndLog(registerUserMessage);


        private async Task SafeCloseAsync()
        {
            try
            {
                if (_channel is not null)
                {
                    await _channel.CloseAsync();
                    await _channel.DisposeAsync();
                    _channel = null;
                }
            }
            catch { /* ignore */ }

            try
            {
                if (_connection is not null)
                {
                    await _connection.CloseAsync();
                    await _connection.DisposeAsync();
                    _connection = null;
                }
            }
            catch { /* ignore */ }
        }
    }
}

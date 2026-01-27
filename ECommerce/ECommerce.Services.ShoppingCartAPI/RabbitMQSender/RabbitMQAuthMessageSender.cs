using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace ECommerce.Services.ShoppingCartAPI.RabbitMQSender
{
    public class RabbitMQAuthMessageSender : IRabbitMQAuthMessageSender
    {
        private readonly IRabbitMqConnectionProvider _connectionProvider;
        private readonly RabbitMqOptions _options;

        public RabbitMQAuthMessageSender(IRabbitMqConnectionProvider connectionProvider,
        IOptions<RabbitMqOptions> options)
        {
            _connectionProvider = connectionProvider;
            _options = options.Value;
        }

        public async Task SendMessageAsync<T>(T message, string queueName, CancellationToken ct = default)
        {
            var connection = await _connectionProvider.GetConnectionAsync(ct);

            // Enable confirms at channel creation time
            CreateChannelOptions? channelOptions = null;
            if (_options.UsePublisherConfirms)
            {
                channelOptions = new CreateChannelOptions(
                    publisherConfirmationsEnabled: true,
                    publisherConfirmationTrackingEnabled: true
                );
            }

            await using var channel = channelOptions is null
                ? await connection.CreateChannelAsync()
                : await connection.CreateChannelAsync(channelOptions);

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: _options.DurableQueues,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: ct);

            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            var props = new BasicProperties
            {
                Persistent = _options.DurableQueues,
                ContentType = "application/json"
            };

            // If confirms are enabled, awaiting BasicPublishAsync waits for the broker confirm.
            // Use CancellationToken to enforce a timeout.
            if (_options.UsePublisherConfirms)
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                cts.CancelAfter(TimeSpan.FromSeconds(5)); // confirm timeout

                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: queueName,
                    mandatory: false,
                    basicProperties: props,
                    body: body,
                    cancellationToken: cts.Token);
            }
            else
            {
                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: queueName,
                    mandatory: false,
                    basicProperties: props,
                    body: body,
                    cancellationToken: ct);
            }
        }
    }
}

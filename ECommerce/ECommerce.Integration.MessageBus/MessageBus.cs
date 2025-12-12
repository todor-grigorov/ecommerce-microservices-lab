using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace ECommerce.Integration.MessageBus
{
    public class MessageBus : IMessageBus
    {
        public async Task PublishMessageAsync<T>(string connectionString, T message, string topic_queue_Name)
        {
            await using var client = new ServiceBusClient(connectionString);

            ServiceBusSender sender = client.CreateSender(topic_queue_Name);

            var jsonMessage = JsonConvert.SerializeObject(message);
            ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString(),
            };

            await sender.SendMessageAsync(finalMessage);
            await client.DisposeAsync();
        }
    }
}

using ECommerce.Integration.MessageBus;
using ECommerce.Services.ShoppingCartAPI.Service.IService;

namespace ECommerce.Services.ShoppingCartAPI.Service
{
    public class ServiceBus : IServiceBus
    {
        private readonly string _connectionString;
        private readonly IMessageBus _messageBus;

        public ServiceBus(string connectionString, IMessageBus messageBus)
        {
            _connectionString = connectionString;
            _messageBus = messageBus;
        }

        public async Task PublishMessageAsync<T>(T message, string topic_queue_Name)
        {
            await _messageBus.PublishMessageAsync(_connectionString, message, topic_queue_Name);
        }
    }
}

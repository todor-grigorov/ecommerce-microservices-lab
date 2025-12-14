using ECommerce.Integration.MessageBus;
using ECommerce.Services.IdentityAPI.Service.IService;

namespace ECommerce.Services.IdentityAPI.Service
{
    public class ServiceBus : IServiceBus
    {
        private readonly IConfiguration _configuration;
        private readonly string? _connectionString;
        private readonly IMessageBus _messageBus;

        public ServiceBus(IConfiguration configuration, IMessageBus messageBus)
        {
            _configuration = configuration;
            _messageBus = messageBus;
            _connectionString = _configuration.GetConnectionString("ServiceBusConnection");
        }

        public async Task PublishMessageAsync<T>(T message, string topic_queue_Name)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Service Bus connection string is not configured.");
            }

            await _messageBus.PublishMessageAsync(_connectionString, message, topic_queue_Name);
        }
    }
}

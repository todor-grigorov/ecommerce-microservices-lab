using RabbitMQ.Client;

namespace ECommerce.Services.IdentityAPI.RabbitMQSender
{
    public interface IRabbitMqConnectionProvider
    {
        Task<IConnection> GetConnectionAsync(CancellationToken ct = default);
    }
}

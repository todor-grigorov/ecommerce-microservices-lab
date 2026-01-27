using RabbitMQ.Client;

namespace ECommerce.Services.ShoppingCartAPI.RabbitMQSender
{
    public interface IRabbitMqConnectionProvider
    {
        Task<IConnection> GetConnectionAsync(CancellationToken ct = default);
    }
}

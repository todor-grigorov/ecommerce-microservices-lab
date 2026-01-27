namespace ECommerce.Services.ShoppingCartAPI.RabbitMQSender
{
    public interface IRabbitMQAuthMessageSender
    {
        Task SendMessageAsync<T>(T message, string queueName, CancellationToken ct = default);
    }
}

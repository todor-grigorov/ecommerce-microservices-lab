namespace ECommerce.Services.IdentityAPI.RabbitMQSender
{
    public interface IRabbitMQAuthMessageSender
    {
        Task SendMessageAsync<T>(T message, string queueName);
    }
}

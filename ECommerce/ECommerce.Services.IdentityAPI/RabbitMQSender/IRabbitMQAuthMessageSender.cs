namespace ECommerce.Services.IdentityAPI.RabbitMQSender
{
    public interface IRabbitMQAuthMessageSender
    {
        void SendMessage<T>(T message, string queueName);
    }
}

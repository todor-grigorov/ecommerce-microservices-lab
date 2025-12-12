namespace ECommerce.Integration.MessageBus
{
    public interface IMessageBus
    {
        Task PublishMessageAsync<T>(T message, string topic_queue_Name);
    }
}

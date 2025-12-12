namespace ECommerce.Integration.MessageBus
{
    public interface IMessageBus
    {
        Task PublishMessageAsync<T>(string connectionString, T message, string topic_queue_Name);
    }
}

namespace ECommerce.Services.IdentityAPI.Service.IService
{
    public interface IServiceBus
    {
        Task PublishMessageAsync<T>(T message, string topic_queue_Name);
    }
}

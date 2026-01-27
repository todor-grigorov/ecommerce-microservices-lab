using ECommerce.Services.EmailAPI.Dto;
using ECommerce.Services.EmailAPI.Service.IService;
using Newtonsoft.Json;
using System.Text;

namespace ECommerce.Services.EmailAPI.Messaging
{
    public sealed class RabbitMQCartConsumer : RabbitMqQueueConsumerBase<CartDto>
    {
        public RabbitMQCartConsumer(
        IConfiguration configuration,
        IServiceScopeFactory scopeFactory,
        ILogger<RabbitMQCartConsumer> logger)
        : base(configuration, scopeFactory, logger) { }

        protected override string QueueConfigKey => "TopicAndQueueNames:EmailShoppingCartQueue";

        protected override CartDto Deserialize(ReadOnlyMemory<byte> body)
            => JsonConvert.DeserializeObject<CartDto>(Encoding.UTF8.GetString(body.Span))!;

        protected override Task HandleAsync(IServiceProvider sp, CartDto message, CancellationToken ct)
            => sp.GetRequiredService<IEmailService>().EmailCartAndLog(message);
    }
}

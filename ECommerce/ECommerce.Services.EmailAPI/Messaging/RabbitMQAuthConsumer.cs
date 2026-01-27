using ECommerce.Services.EmailAPI.Service.IService;
using Newtonsoft.Json;
using System.Text;

namespace ECommerce.Services.EmailAPI.Messaging
{
    public sealed class RabbitMQAuthConsumer : RabbitMqQueueConsumerBase<string>
    {

        public RabbitMQAuthConsumer(IConfiguration configuration,
        IServiceScopeFactory scopeFactory,
        ILogger<RabbitMQAuthConsumer> logger)
        : base(configuration, scopeFactory, logger) { }

        protected override string QueueConfigKey => "TopicAndQueueNames:RegisterUserQueue";

        protected override string Deserialize(ReadOnlyMemory<byte> body)
            => JsonConvert.DeserializeObject<string>(Encoding.UTF8.GetString(body.Span))!;

        protected override Task HandleAsync(IServiceProvider sp, string message, CancellationToken ct)
            => sp.GetRequiredService<IEmailService>().RegisterUserEmailAndLog(message);
    }
}

using Azure.Messaging.ServiceBus;
using ECommerce.Services.EmailAPI.Service.IService;
using ECommerce.Services.RewardsApi.Dto;
using Newtonsoft.Json;
using System.Text;

namespace ECommerce.Services.RewardsApi.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string orderCreatedTopic;
        private readonly string orderCreatedRewardsSubscription;
        private readonly IConfiguration _configuration;
        private readonly IRewardsService _rewardsService;

        private readonly ServiceBusProcessor _rewardsProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, IRewardsService rewardsService)
        {
            _rewardsService = rewardsService;
            _configuration = configuration;
            serviceBusConnectionString = _configuration.GetConnectionString("ServiceBusConnection") ?? throw new InvalidOperationException("ServiceBusConnection string is missing.");
            orderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic") ?? throw new InvalidOperationException("OrderCreatedTopic is missing.");
            orderCreatedRewardsSubscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedRewardsUpdate") ?? throw new InvalidOperationException("OrderCreatedRewardsUpdate is missing.");


            var client = new ServiceBusClient(serviceBusConnectionString);
            _rewardsProcessor = client.CreateProcessor(orderCreatedTopic, orderCreatedRewardsSubscription);
        }

        public async Task Start()
        {
            await StartEmailCartProcessor();

        }
        public async Task Stop()
        {
            await StopEmailCartProcessor();
        }


        private async Task StartEmailCartProcessor()
        {
            _rewardsProcessor.ProcessMessageAsync += OnNewOrderRewardsRequestReceived;
            _rewardsProcessor.ProcessErrorAsync += ErrorHandler;
            await _rewardsProcessor.StartProcessingAsync();
        }

        private async Task StopEmailCartProcessor()
        {
            await _rewardsProcessor.StopProcessingAsync();
            await _rewardsProcessor.DisposeAsync();
        }

        private async Task OnNewOrderRewardsRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            RewardsMessageDto rewardsMessage = JsonConvert.DeserializeObject<RewardsMessageDto>(body);

            try
            {
                // TODO: Implement email sending logic here using cartDto
                await _rewardsService.UpdateRewards(rewardsMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}

using Azure.Messaging.ServiceBus;
using ECommerce.Services.EmailAPI.Dto;
using ECommerce.Services.EmailAPI.Service.IService;
using Newtonsoft.Json;
using System.Text;

namespace ECommerce.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly string registerUserQueue;
        private readonly string orderCreatedTopic;
        private readonly string orderCreatedEmailSubscription;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        private ServiceBusProcessor _emailOrderPlacedProcessor;
        private ServiceBusProcessor _emailCartProcessor;
        private ServiceBusProcessor _registerUserProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, IEmailService emailService)
        {
            _emailService = emailService;
            _configuration = configuration;
            serviceBusConnectionString = _configuration.GetConnectionString("ServiceBusConnection");
            emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue")!;
            registerUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue")!;
            orderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic")!;
            orderCreatedEmailSubscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedEmail")!;


            var client = new ServiceBusClient(serviceBusConnectionString);
            _emailCartProcessor = client.CreateProcessor(emailCartQueue);
            _registerUserProcessor = client.CreateProcessor(registerUserQueue);
            _emailOrderPlacedProcessor = client.CreateProcessor(orderCreatedTopic, orderCreatedEmailSubscription);

        }

        public async Task Start()
        {
            await StartEmailCartProcessor();
            await StartRegisterUserProcessor();
            await StartEmailOrderPlacedProcessor();

        }
        public async Task Stop()
        {
            await StopEmailCartProcessor();
            await StopRegisterUserProcessor();
            await StopEmailOrderPlacedProcessor();
        }


        private async Task StartEmailCartProcessor()
        {
            _emailCartProcessor.ProcessMessageAsync += OnEmailCartReceived;
            _emailCartProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailCartProcessor.StartProcessingAsync();
        }

        private async Task StopEmailCartProcessor()
        {
            await _emailCartProcessor.StopProcessingAsync();
            await _emailCartProcessor.DisposeAsync();
        }

        private async Task StartRegisterUserProcessor()
        {
            _registerUserProcessor.ProcessMessageAsync += OnRegisterUserReceived;
            _registerUserProcessor.ProcessErrorAsync += ErrorHandler;
            await _registerUserProcessor.StartProcessingAsync();
        }
        private async Task StopRegisterUserProcessor()
        {
            await _registerUserProcessor.StopProcessingAsync();
            await _registerUserProcessor.DisposeAsync();
        }

        private async Task StartEmailOrderPlacedProcessor()
        {
            _emailOrderPlacedProcessor.ProcessMessageAsync += OnOrderPlacedReceived;
            _emailOrderPlacedProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailOrderPlacedProcessor.StartProcessingAsync();
        }

        private async Task OnOrderPlacedReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            RewardsMessageDto objMessage = JsonConvert.DeserializeObject<RewardsMessageDto>(body);

            try
            {
                //TODO - try to log email
                await _emailService.LogOrderPlaced(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task StopEmailOrderPlacedProcessor()
        {
            await _emailOrderPlacedProcessor.StopProcessingAsync();
            await _emailOrderPlacedProcessor.DisposeAsync();
        }

        private async Task OnRegisterUserReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            string userEmail = JsonConvert.DeserializeObject<string>(body);

            try
            {
                // TODO: Implement email sending logic here using cartDto
                await _emailService.RegisterUserEmailAndLog(userEmail);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task OnEmailCartReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(body);

            try
            {
                // TODO: Implement email sending logic here using cartDto
                await _emailService.EmailCartAndLog(cartDto);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
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

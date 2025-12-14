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
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private ServiceBusProcessor _registerUserProcessor;

        private readonly ServiceBusProcessor _emailCartProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, IEmailService emailService)
        {
            _emailService = emailService;
            _configuration = configuration;
            serviceBusConnectionString = configuration.GetConnectionString("ServiceBusConnection");
            emailCartQueue = configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
            registerUserQueue = configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue");


            var client = new ServiceBusClient(serviceBusConnectionString);
            _emailCartProcessor = client.CreateProcessor(emailCartQueue);
            _registerUserProcessor = client.CreateProcessor(registerUserQueue);
        }

        public async Task Start()
        {
            await StartEmailCartProcessor();
            await StartRegisterUserProcessor();

        }
        public async Task Stop()
        {
            await StopEmailCartProcessor();
            await StopRegisterUserProcessor();
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

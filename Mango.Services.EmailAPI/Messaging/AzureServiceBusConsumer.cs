using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Models.Dto;
using Mango.Services.EmailAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer :IAzureServiceBusConsumer
    {
        private readonly string serviceBusCon;
        private readonly string emailCartQueue;
        private readonly IConfiguration configuration;
        private readonly EmailService emailService;
        private ServiceBusProcessor _emailCartProcessor;
        public AzureServiceBusConsumer(IConfiguration configuration,EmailService emailService)
        {
            this.configuration = configuration;
            this.emailService = emailService;
            serviceBusCon = configuration.GetValue<string>("ServiceBusConnectionString");
            emailCartQueue = configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
            var client = new ServiceBusClient(serviceBusCon);
            _emailCartProcessor = client.CreateProcessor(emailCartQueue);
        }

        public async Task Start()
        {
            _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _emailCartProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailCartProcessor.StartProcessingAsync();

        }

        public async Task Stop()
        {
            await _emailCartProcessor.StopProcessingAsync();
            await _emailCartProcessor.DisposeAsync();
        }
        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            //this is where you will receive message
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CartDto objMessage = JsonConvert.DeserializeObject<CartDto>(body);
            try
            {
                //TODO - try to log email
                await emailService.EmailCartAndLog(objMessage);
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

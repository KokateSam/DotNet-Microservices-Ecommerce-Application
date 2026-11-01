using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Models.Dto;
using Mango.Services.EmailAPI.Services;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer:IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly IConfiguration _configuration;
        private ServiceBusProcessor _emailCartProcessor;
        private readonly EmailService _emailService;
        public AzureServiceBusConsumer(IConfiguration configuration,EmailService emailService)
        { 
            _emailService=emailService;
           _configuration = configuration;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            emailCartQueue = _configuration.GetValue<string>("TopicAndQueNames:EmailShoppingCartQueue");

            var client = new ServiceBusClient(serviceBusConnectionString);

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
           _emailCartProcessor.StopProcessingAsync();
           _emailCartProcessor.DisposeAsync();

        }
        private  Task ErrorHandler(ProcessErrorEventArgs args)
        {
            // if any error occured will be received here and may send to user 

            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            //your message receive here

            var message= args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            CartDto messegeObj=JsonConvert.DeserializeObject<CartDto>(body);
            try
            {
                //send and log email
                await _emailService.EmailCartAndLog(messegeObj);

                //here comlete the message delivery and message will be removed from Azure Queue
                await args.CompleteMessageAsync(args.Message);

            }
            catch (Exception ex)
            {
                throw;
            }

        }



        
    }
}

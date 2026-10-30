using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Models.Dto;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer:IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emaiCartQueue;
        private readonly IConfiguration _configuration;
        private ServiceBusProcessor _emailCartProcessor;
        public AzureServiceBusConsumer(IConfiguration configuration)
        { 
           _configuration = configuration;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            emaiCartQueue = _configuration.GetValue<string>("TopicAndQueNames:EmailShoppingCartQueue");

            var client = new ServiceBusClient(serviceBusConnectionString);

            _emailCartProcessor = client.CreateProcessor(emaiCartQueue);
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
                //will do it later-try to log email
                await args.CompleteMessageAsync(args.Message);

            }
            catch (Exception ex)
            {
                throw;
            }

        }



        
    }
}

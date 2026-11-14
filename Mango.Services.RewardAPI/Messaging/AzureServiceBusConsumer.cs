using Azure.Messaging.ServiceBus;
using Mango.Services.RewardAPI.Message;
using Mango.Services.RewardAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.RewardAPI.Messaging
{
    public class AzureServiceBusConsumer:IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string OrderCreatedTopic;
        private readonly string OrderCreatedRewardSubscription;
        private readonly IConfiguration _configuration; 
        private readonly RewardService _rewardService;
        private ServiceBusProcessor _rewardProcessor;
        public AzureServiceBusConsumer(IConfiguration configuration,RewardService rewardService)
        { 
            _rewardService=rewardService;
           _configuration = configuration;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            OrderCreatedTopic = _configuration.GetValue<string>("TopicAndQueNames:Azure_Portal_MessageBus_Topic_Name_you_created");
            OrderCreatedRewardSubscription = _configuration.GetValue<string>("TopicAndQueNames:Azure_Portal_MessageBus_Subscription_Name_you_created");

            var client = new ServiceBusClient(serviceBusConnectionString);

            _rewardProcessor = client.CreateProcessor(OrderCreatedTopic, OrderCreatedRewardSubscription);

        }
        public async Task Start()
        {
            _rewardProcessor.ProcessMessageAsync += OnNewOrderRewardsRequestReceived;
            _rewardProcessor.ProcessErrorAsync += ErrorHandler;
            await _rewardProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
           await _rewardProcessor.StopProcessingAsync();
           await _rewardProcessor.DisposeAsync();

        }    
        private async Task OnNewOrderRewardsRequestReceived(ProcessMessageEventArgs args)
        {
            //your message receive here

            var message= args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            RewardMessage messegeObj=JsonConvert.DeserializeObject<RewardMessage>(body);
            try
            {
                //send and log email
                await _rewardService.UpdateRewards(messegeObj);

                //here comlete the message delivery and message will be removed from Azure Queue
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            // if any error occured will be received here and may send to user 

            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

    }
}

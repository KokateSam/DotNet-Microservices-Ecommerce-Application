using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mango.MessageBus
{
    public class MessageBus : IMessageBus
    {
        // private string connectionString = "AzurePortal_Servicebus_YourServiceBus_SharedAccesspolicy_sharedAccesskey_PrimaryConnectionstring";
        //replace this with above setting from Azure portal
        private string connectionString = "";

        public async Task PublishMessage(object message, string topic_queu_name)
        {

            await using var client = new ServiceBusClient(connectionString);
            ServiceBusSender sender = client.CreateSender(topic_queu_name);

            var jsonmessage=JsonConvert.SerializeObject(message);
            ServiceBusMessage finalmessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonmessage))
            {
                CorrelationId = Guid.NewGuid().ToString()
            };
            await sender.SendMessageAsync(finalmessage);
            await client.DisposeAsync();
        }
    }
}

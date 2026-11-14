
using Mango.Services.RewardAPI.Messaging;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Mango.Services.RewardAPI.Extension
{
    public static class ApplicationBuilderExtensions
    {
        private static IAzureServiceBusConsumer serviceBusConsumer { get; set; }
        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            serviceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
            var hostApplicationlife = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            hostApplicationlife.ApplicationStarted.Register(OnStart);
            hostApplicationlife.ApplicationStopped.Register(OnStop);

            return app;
        }
        private static void OnStop()
        {
            serviceBusConsumer.Stop();
        }
        private static void OnStart()
        {
            serviceBusConsumer?.Start();
        }
    }
}

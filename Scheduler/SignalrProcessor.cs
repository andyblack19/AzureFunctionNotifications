using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace Scheduler
{
    public static class SignalrProcessor
    {
        private const string FunctionName = "SignalrProcessor";
        private const string InfrastructureStorageAppSetting = "BRIGHTHR_STORAGE_INFRASTRUCTURE";
        private const string SignalrConnectionAppSetting = "AzureSignalRConnectionString";
        private const string InputQueueName = "notification-signalr";
        private const string HubName = "andy";

        [FunctionName(FunctionName)]
        public static async Task Run(
            [QueueTrigger(InputQueueName, Connection = InfrastructureStorageAppSetting)] SignalrNotification notification,
            [SignalR(HubName = HubName, ConnectionStringSetting = SignalrConnectionAppSetting)] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    UserId = $"{notification.UserGuid}",
                    Target = "clientFunctionName",
                    Arguments = new object[] { notification.Message }
                });
        }
    }
}

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace Scheduler
{
    public static class SignalrSubscriber
    {
        private const string FunctionName = "SignalrSubscriber";
        private const string SignalrConnectionAppSetting = "AzureSignalRConnectionString";
        private const string HubName = "andy";

        [FunctionName(FunctionName)]
        public static SignalRConnectionInfo Run(
            [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest request,
            [SignalRConnectionInfo(HubName = HubName, ConnectionStringSetting = SignalrConnectionAppSetting, UserId = "{headers.x-bright-user-id}")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }
    }
}

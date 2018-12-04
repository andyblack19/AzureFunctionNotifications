using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Scheduler
{
    public static class Birthday
    {
        [FunctionName("Birthday")]
        public static async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo timer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}

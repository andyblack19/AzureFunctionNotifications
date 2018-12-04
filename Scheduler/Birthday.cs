using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace Scheduler
{
    public static class Birthday
    {
        private const string FunctionName = "Birthday";

        [FunctionName(FunctionName)]
        public static async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo timer)
        {
            Logging.Success($"Timer trigger function executed at UTC: {DateTime.UtcNow}", FunctionName);
        }
    }
}

using System;
using Microsoft.Azure.WebJobs;

namespace Scheduler
{
    public static class NotificationProcessor
    {
        private const string FunctionName = "NotificationProcessor";
        private const string InputQueueName = "notification-processor";
        private const string InfrastructureStorageAppSetting = "BRIGHTHR_STORAGE_INFRASTRUCTURE";

        [FunctionName(FunctionName)]
        public static void Run(
            [QueueTrigger(InputQueueName, Connection = InfrastructureStorageAppSetting)] Notification notification)
        {
            Logging.Success(
                $"Queue trigger function executed at UTC: {DateTime.UtcNow}", FunctionName);
        }
    }
}

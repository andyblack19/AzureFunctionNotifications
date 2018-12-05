using Microsoft.Azure.WebJobs;
using SendGrid.Helpers.Mail;

namespace Scheduler
{
    public static class EmailProcessor
    {
        private const string FunctionName = "EmailProcessor";
        private const string InfrastructureStorageAppSetting = "BRIGHTHR_STORAGE_INFRASTRUCTURE";
        private const string SendGridAppSetting = "BRIGHTHR_SENDGRID_APIKEY";
        private const string InputQueueName = "notification-email";

        [FunctionName(FunctionName)]
        public static void Run(
            [QueueTrigger(InputQueueName, Connection = InfrastructureStorageAppSetting)] EmailNotification notification,
            [SendGrid(ApiKey = SendGridAppSetting)] out SendGridMessage message)
        {
            Logging.Success("C# Email queue trigger function processed", FunctionName);

            message = new SendGridMessage
            {
                From = new EmailAddress("noreply@brighthr.com", "Bright HR"),
                Subject = notification.Subject,
                HtmlContent = notification.Body,
                TemplateId = "0f3247f7-27d9-4b43-af54-be9d22e29cab",
            };
            message.AddTo(notification.To);
        }
    }
}

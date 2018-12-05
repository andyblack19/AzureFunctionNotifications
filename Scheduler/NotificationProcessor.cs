using System;
using System.Linq;
using System.Net.Mail;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;

namespace Scheduler
{
    public static class NotificationProcessor
    {
        private const string FunctionName = "NotificationProcessor";
        private const string InfrastructureStorageAppSetting = "BRIGHTHR_STORAGE_INFRASTRUCTURE";
        private const string InputQueueName = "notification-processor";
        private const string EmailQueueName = "notification-email";
        private const string AndroidPushNotificationQueueName = "notification-android";
        private const string IosPushNotificationQueueName = "notification-ios";
        private const string SignalrNotificationQueueName = "notification-signalr";

        [FunctionName(FunctionName)]
        public static void Run(
            [QueueTrigger(InputQueueName, Connection = InfrastructureStorageAppSetting)] Notification notification,
            [Queue(EmailQueueName, Connection = InfrastructureStorageAppSetting)] ICollector<EmailNotification> email,
            [Queue(AndroidPushNotificationQueueName, Connection = InfrastructureStorageAppSetting)] ICollector<PushNotification> androidPush,
            [Queue(IosPushNotificationQueueName, Connection = InfrastructureStorageAppSetting)]  ICollector<PushNotification> iosPush,
            [Queue(SignalrNotificationQueueName, Connection = InfrastructureStorageAppSetting)]  ICollector<PushNotification> webPush)
        {
            Logging.Success($"Queue trigger function executed at UTC: {DateTime.UtcNow}", FunctionName);

            foreach (var recipient in notification.EmailRecipients)
            {
                email.Add(new EmailNotification(new MailAddress(recipient), notification.EmailSubject, notification.EmailHtmlContent));
            }

            foreach (var device in notification.DeviceRecipients.Where(x => x.Platform == "android"))
            {
                androidPush.Add(new PushNotification(device.Token, notification.PushNotificationContent));
            }

            foreach (var device in notification.DeviceRecipients.Where(x => x.Platform == "ios"))
            {
                iosPush.Add(new PushNotification(device.Token, notification.PushNotificationContent));
            }

            foreach (var device in notification.DeviceRecipients.Where(x => x.Platform == "web"))
            {
                webPush.Add(new PushNotification(device.Token, notification.PushNotificationContent));
            }
        }

        public class EmailNotification
        {
            public MailAddress From => new MailAddress("noreply@brighthr.com");
            public MailAddress To { get; }
            public string Subject { get; }
            public string Body { get; }

            [JsonConstructor]
            private EmailNotification()
            {
            }

            public EmailNotification(MailAddress to, string subject, string body)
            {
                To = to;
                Subject = subject;
                Body = body;
            }
        }

        public class PushNotification
        {
            public string Device { get; }
            public string Message { get; }

            [JsonConstructor]
            private PushNotification()
            {
            }

            public PushNotification(string device, string message)
            {
                Device = device;
                Message = message;
            }
        }
    }
}

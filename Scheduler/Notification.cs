using System.Collections.Generic;
using Newtonsoft.Json;

namespace Scheduler
{
    public class Notification
    {
        [JsonConstructor]
        private Notification()
        {
        }

        protected Notification(
            int type,
            IEnumerable<string> emailRecipients,
            string htmlEmailContent,
            IEnumerable<string> deviceRecipients,
            string pushNotificationContent)
        {
            Type = type;
            EmailRecipients = emailRecipients;
            HtmlEmailContent = htmlEmailContent;
            DeviceRecipients = deviceRecipients;
            PushNotificationContent = pushNotificationContent;
        }

        public int Type { get; }

        public IEnumerable<string> EmailRecipients { get; }
        public string HtmlEmailContent { get; }

        public IEnumerable<string> DeviceRecipients { get; }
        public string PushNotificationContent { get; }
    }

    public class BirthdayNotification : Notification
    {
        public BirthdayNotification(Employee employee) :
            base(7,
                new List<string> {employee.EmailAddress},
                $"<h2>Hi {employee.Personal.FirstName},</h2><br/><br/><strong>Happy Birthday</strong> from everyone at BrightHR.",
                employee.DeviceTokens,
                "Happy Birthday! from BrightHR")
        {
        }
    }
}
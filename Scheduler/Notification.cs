using System;
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
            Guid userGuid,
            int type,
            IEnumerable<string> emailRecipients,
            string emailSubject,
            string emailHtmlContent,
            IEnumerable<Device> deviceRecipients,
            string pushNotificationContent)
        {
            UserGuid = userGuid;
            Type = type;
            EmailRecipients = emailRecipients;
            EmailSubject = emailSubject;
            EmailHtmlContent = emailHtmlContent;
            DeviceRecipients = deviceRecipients;
            PushNotificationContent = pushNotificationContent;
        }

        public Guid UserGuid { get; set; }
        public int Type { get; set; }

        public IEnumerable<string> EmailRecipients { get; set; }
        public string EmailSubject { get; set; }
        public string EmailHtmlContent { get; set; }

        public IEnumerable<Device> DeviceRecipients { get; set; }
        public string PushNotificationContent { get; set; }
    }

    public class BirthdayNotification : Notification
    {
        public BirthdayNotification(Employee employee) :
            base(new Guid("59200E96-CB00-4FFD-856D-BE23786FEEC7"), 
                7,
                new List<string> {employee.EmailAddress},
                "subject - happy birthday",
                $"<h2>Hi {employee.Personal.FirstName},</h2><br/><br/><strong>Happy Birthday</strong> from everyone at BrightHR.",
                employee.DeviceTokens,
                "Happy Birthday! from BrightHR")
        {
        }
    }
}
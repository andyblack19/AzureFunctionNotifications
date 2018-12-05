using System.Collections.Generic;

namespace Scheduler
{
    public abstract class Notification
    {
        public abstract IEnumerable<string> EmailRecipients { get; }
        public abstract string HtmlEmailContent { get; }

        public abstract IEnumerable<string> DeviceRecipients { get; }
        public abstract string PushNotificationContent { get; }
    }

    public class BirthdayNotification : Notification
    {
        public override IEnumerable<string> EmailRecipients { get; }
        public override string HtmlEmailContent { get; }

        public override IEnumerable<string> DeviceRecipients { get; }
        public override string PushNotificationContent => "Happy Birthday! from BrightHR";

        public BirthdayNotification(Employee employee)
        {
            EmailRecipients = new List<string> { employee.EmailAddress };

            HtmlEmailContent = $"<h2>Hi {employee.Personal.FirstName},</h2><br/><br/><strong>Happy Birthday</strong> from everyone at BrightHR.";

            DeviceRecipients = employee.DeviceTokens;
        }
    }
}
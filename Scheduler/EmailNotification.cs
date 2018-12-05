using Newtonsoft.Json;

namespace Scheduler
{
    public class EmailNotification
    {
        public string From => "noreply@brighthr.com";
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        [JsonConstructor]
        private EmailNotification()
        {
        }

        public EmailNotification(string to, string subject, string body)
        {
            To = to;
            Subject = subject;
            Body = body;
        }
    }
}
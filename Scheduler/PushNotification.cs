using Newtonsoft.Json;

namespace Scheduler
{
    public class PushNotification
    {
        public string Device { get; set; }
        public string Message { get; set; }

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
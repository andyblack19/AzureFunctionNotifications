using System;
using Newtonsoft.Json;

namespace Scheduler
{
    public class SignalrNotification
    {
        public Guid UserGuid { get; set; }
        public string Message { get; set; }

        [JsonConstructor]
        private SignalrNotification()
        {
        }

        public SignalrNotification(Guid userGuid, string message)
        {
            UserGuid = userGuid;
            Message = message;
        }
    }
}
using System;
using System.Collections.Generic;

namespace Scheduler
{
    public class Employee
    {
        public long Id { get; set; }
        public Guid UserGuid { get; set; }
        public string EmailAddress { get; set; }
        public Personal Personal { get; set; }
        public IEnumerable<Device> DeviceTokens { get; set; }
    }

    public class Personal
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class Device
    {
        public string Token { get; set; }
        public string Platform { get; set; }
    }
}
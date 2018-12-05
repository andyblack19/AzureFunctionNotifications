using System;
using System.Collections.Generic;

namespace Scheduler
{
    public class Employee
    {
        public long Id { get; set; }
        public Guid UserGuid { get; set; }
        public string EmailAddress { get; set; }
        public IEnumerable<string> DeviceTokens { get; set; }

        public Personal Personal { get; set; }
    }

    public class Personal
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Azure.WebJobs;

namespace Scheduler
{
    public static class Birthday
    {
        private const string FunctionName = "Birthday";
        private const string OutputQueueName = "notification-processor";
        private const string InfrastructureStorageAppSetting = "BRIGHTHR_STORAGE_INFRASTRUCTURE";

        [FunctionName(FunctionName)]
        public static async Task Run(
            [TimerTrigger("*/30 * * * * *")]TimerInfo trigger,
            [Queue(OutputQueueName, Connection = InfrastructureStorageAppSetting)] ICollector<Notification> notifications)
        {
            Logging.Success($"Timer trigger function executed at UTC: {DateTime.UtcNow}", FunctionName);

            foreach (var employee in await FetchTheData())
            {
                notifications.Add(new BirthdayNotification(employee));
            }
        }

        private static async Task<IEnumerable<Employee>> FetchTheData()
        {
            var connectionString = Environment.GetEnvironmentVariable("BRIGHTHR_DB_GB");
            var connection = new SqlConnection(connectionString);

            var dynamic = await connection.QueryAsync(
                "SELECT * FROM [User] WHERE UserGuid = @id",
                new { id = new Guid("6B7C2B96-6C83-438F-84C6-5428F8D8D960") });

            var typed = await connection.QueryAsync<Employee>(
                "SELECT * FROM [User] WHERE UserGuid = @id",
                new { id = new Guid("6B7C2B96-6C83-438F-84C6-5428F8D8D960") });

            var multi = await connection.QueryAsync<Employee, Personal, Employee>(
                "SELECT u.*, p.* FROM [User] u INNER JOIN [PersonalDetails] p ON u.PersonalDetailsId = p.Id WHERE u.UserGuid = @id",
                (employee, personal) =>
                {
                    employee.Personal = personal;
                    return employee;
                },
                param: new { id = new Guid("6B7C2B96-6C83-438F-84C6-5428F8D8D960") },
                splitOn: "Id");

            var result = new Employee
            {
                Id = 1,
                UserGuid = Guid.NewGuid(),
                EmailAddress = "andyblack16@gmail.com",
                DeviceTokens = new List<Device>
                {
                    //new Device { Platform = "android", Token = "fNLzEiGMdRY:APA91bEjzJVsN30LFUV02v3R8tiB-gLbUMfuIttyq_-5Md1sZY_m99y5H5o7WTXKWxa7RpmGstI8AzLYHagee5pjPiPU6_eGiP6SRM8nvxXMtgJzN6drDDdoUD0GVmOTkb-28td1dFmC52KZA5ViHN1PKLWyQcPfOA" },
                    //new Device { Platform = "android", Token = "en1gPUT2QU4:APA91bEbuE-9COuZ5Dndp4qkj7LeIc077LrLAIsZ4wSJhJO8zEgqw5ka4EdtBFQ79hk4eob-WJ5n_OgULDNjbl-gXmodkARFIf8cIzHmiMOCvWt1F2vc1jIRnratxgfxtFGvbPPKh9Fks_u4jNRrYTsyA3-WPcdHcg" },
                    //new Device { Platform = "android", Token = "dWkqrtpYa_8:APA91bEFQZIQrK0y-5kkzQeC-xGbJH1r6lDV5voznIMqB2r-6Rd69iptInx-LNDkgKK9NYlo_vpxVkb5Kp2qax57JV10RB_PTilYwW_-oasW29UjKOXDUbMSbtgzy4YbgX3ZeMF48l5R" },
                    new Device { Platform = "android", Token = "eSOPyaY3PtU:APA91bHyfJyMZxjDlq7Xd4y3j3PeogJ8ubhhZ0MjP1sBylWWT0NzFVgVnJXf6crBD8g8gLJJhPKIn9uKfZ002nRze6FiMPeKbbhCYIrhHfyRKlUXF71NeUsqpNaNlJnMwW3mS4uLS_F0" },
                },
                Personal = new Personal
                {
                    FirstName = "Andy",
                    LastName = "Black"
                }
            };

            return new[] { result };
        }
    }
}

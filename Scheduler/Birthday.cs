using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Azure.WebJobs;

namespace Scheduler
{
    public static class Birthday
    {
        private const string FunctionName = "Birthday";

        [FunctionName(FunctionName)]
        public static async Task Run([TimerTrigger("*/10 * * * * *")]TimerInfo timer)
        {
            Logging.Success($"Timer trigger function executed at UTC: {DateTime.UtcNow}", FunctionName);

            var connectionString = Environment.GetEnvironmentVariable("BRIGHTHR_DB_GB");
            var connection = new SqlConnection(connectionString);

            var dynamic = connection.Query(
                "SELECT * FROM [User] WHERE UserGuid = @id",
                new { id = new Guid("6B7C2B96-6C83-438F-84C6-5428F8D8D960") });

            var typed = connection.Query<Employee>(
                "SELECT * FROM [User] WHERE UserGuid = @id",
                new { id = new Guid("6B7C2B96-6C83-438F-84C6-5428F8D8D960") });
            
            var multi = connection.Query<Employee, Personal, Employee>(
                "SELECT u.*, p.* FROM [User] u INNER JOIN [PersonalDetails] p ON u.PersonalDetailsId = p.Id WHERE u.UserGuid = @id",
                (employee, personal) => { employee.Personal = personal; return employee; },
                param: new { id = new Guid("6B7C2B96-6C83-438F-84C6-5428F8D8D960") },
                splitOn: "Id");
        }
    }

    public class Employee
    {
        public long Id { get; set; }
        public Guid UserGuid { get; set; }
        public string EmailAddress { get; set; }

        public Personal Personal { get; set; }
    }

    public class Personal
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}

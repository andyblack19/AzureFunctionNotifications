using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json.Linq;
using PushSharp.Apple;
using static PushSharp.Apple.ApnsConfiguration.ApnsServerEnvironment;

namespace Scheduler
{
    public static class AppleProcessor
    {
        private const string FunctionName = "AppleProcessor";
        private const string InfrastructureStorageAppSetting = "BRIGHTHR_STORAGE_INFRASTRUCTURE";
        private const string InputQueueName = "notification-ios";
        private const string ProductionCertThumbprint = "39EB7DC0D71EAE0BB1DD8E24DF2E94F4E116915A";
        private const string SandboxCertThumbprint = "45B4E95A097176B273F90E6F1D8DA2AB16B27E8C";

        [FunctionName(FunctionName)]
        public static void Run(
            [QueueTrigger(InputQueueName, Connection = InfrastructureStorageAppSetting)] PushNotification notification)
        {
            Logging.Success($"Queue trigger function executed at UTC: {DateTime.UtcNow}", FunctionName);

            var apnsBroker = new ApnsServiceBroker(new ApnsConfiguration(Sandbox, GetCertificate()));

            apnsBroker.OnNotificationFailed += (not, aggregateEx) => {

                aggregateEx.Handle(ex => {

                    // See what kind of exception it was to further diagnose
                    if (ex is ApnsNotificationException notificationException)
                    {

                        // Deal with the failed notification
                        var apnsNotification = notificationException.Notification;
                        var statusCode = notificationException.ErrorStatusCode;

                        Console.WriteLine($"Apple Notification Failed: ID={apnsNotification.Identifier}, Code={statusCode}");

                    }
                    else
                    {
                        // Inner exception might hold more useful information like an ApnsConnectionException			
                        Console.WriteLine($"Apple Notification Failed for some unknown reason : {ex.InnerException}");
                    }

                    // Mark it as handled
                    return true;
                });
            };

            apnsBroker.OnNotificationSucceeded += (not) => {
                Console.WriteLine("Apple Notification Sent!");
            };

            apnsBroker.Start();
            
            dynamic alert = new ExpandoObject();
            alert.aps = new ExpandoObject();
            alert.aps.alert = notification.Message;
            alert.aps.sound = "xing.caf";
            alert.aps.type = 7;
            alert.aps.id = 4290;
            alert.aps.notificationGuid = Guid.Parse("59200E96-CB00-4FFD-856D-BE23786FEEC7");
            alert.data = new ExpandoObject();
            apnsBroker.QueueNotification(new ApnsNotification
            {
                DeviceToken = notification.Device,
                Payload = JObject.FromObject((object)alert)
            });
        }

        private static X509Certificate2 GetCertificate()
        {
            var locations = new List<StoreLocation>
            {
                StoreLocation.CurrentUser,
                StoreLocation.LocalMachine,
            };

            foreach (var location in locations)
            {
                var store = new X509Store("My", location);
                try
                {
                    store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                    var certificates = store.Certificates.Find(
                        X509FindType.FindByThumbprint, ProductionCertThumbprint, false);
                    if (certificates.Count > 0)
                    {
                        return certificates[0];
                    }
                }
                finally
                {
                    store.Close();
                }
            }
            throw new ArgumentException($"A Certificate with Thumbprint '{ProductionCertThumbprint}' could not be located.");
        }
    }
}

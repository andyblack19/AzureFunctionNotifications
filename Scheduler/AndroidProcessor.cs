using System;
using System.Collections.Generic;
using System.Dynamic;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json.Linq;
using PushSharp.Core;
using PushSharp.Google;

namespace Scheduler
{
    public static class AndroidProcessor
    {
        private const string FunctionName = "AndroidProcessor";
        private const string InfrastructureStorageAppSetting = "BRIGHTHR_STORAGE_INFRASTRUCTURE";
        private const string InputQueueName = "notification-android";

        [FunctionName(FunctionName)]
        public static void Run(
            [QueueTrigger(InputQueueName, Connection = InfrastructureStorageAppSetting)] PushNotification notification)
        {
            Logging.Success($"Queue trigger function executed at UTC: {DateTime.UtcNow}", FunctionName);

            var gcmBroker = new GcmServiceBroker(new GcmConfiguration("AIzaSyAJPSeYx8u2oqymW9UTsCJI3-b6UZ4HWro"));
            var provider = "GCM";

            gcmBroker.OnNotificationFailed += (not, aggregateEx) => {

                aggregateEx.Handle(ex => {

                    // See what kind of exception it was to further diagnose
                    if (ex is GcmNotificationException notificationException)
                    {

                        // Deal with the failed notification
                        var gcmNotification = notificationException.Notification;
                        var description = notificationException.Description;

                        Logging.Error($"{provider} Notification Failed: ID={gcmNotification.MessageId}, Desc={description}", FunctionName, null);
                    }
                    else if (ex is GcmMulticastResultException multicastException)
                    {

                        foreach (var succeededNotification in multicastException.Succeeded)
                        {
                            Logging.Success($"{provider} Notification Succeeded: ID={succeededNotification.MessageId}", FunctionName);
                        }

                        foreach (var failedKvp in multicastException.Failed)
                        {
                            var n = failedKvp.Key;
                            var e = failedKvp.Value;

                            Logging.Error($"{provider} Notification Failed: ID={n.MessageId}, Desc={e.Message}", FunctionName, null);
                        }

                    }
                    else if (ex is DeviceSubscriptionExpiredException expiredException)
                    {

                        var oldId = expiredException.OldSubscriptionId;
                        var newId = expiredException.NewSubscriptionId;

                        Logging.Error($"Device RegistrationId Expired: {oldId}", FunctionName, null);

                        if (!string.IsNullOrWhiteSpace(newId))
                        {
                            // If this value isn't null, our subscription changed and we should update our database
                            Logging.Error($"Device RegistrationId Changed To: {newId}", FunctionName, null);
                        }
                    }
                    else if (ex is RetryAfterException retryException)
                    {

                        // If you get rate limited, you should stop sending messages until after the RetryAfterUtc date
                        Logging.Error($"{provider} Rate Limited, don't send more until after {retryException.RetryAfterUtc}", FunctionName, null);
                    }
                    else
                    {
                        Logging.Error("{provider} Notification Failed for some unknown reason", FunctionName, null);
                    }

                    // Mark it as handled
                    return true;
                });
            };

            gcmBroker.OnNotificationSucceeded += (not) => {
                Logging.Success($"{provider} Notification Sent!", FunctionName);
            };

            gcmBroker.Start();

            dynamic alert = new ExpandoObject();
            alert.message = notification.Message;
            alert.notificationType = 7;
            alert.id = 4290;
            alert.notificationGuid = Guid.Parse("59200E96-CB00-4FFD-856D-BE23786FEEC7");
            alert.data = new ExpandoObject();
            gcmBroker.QueueNotification(new GcmNotification
            {
                RegistrationIds = new List<string> { notification.Device },
                Data = JObject.FromObject((object)alert)
            });
        }
    }
}

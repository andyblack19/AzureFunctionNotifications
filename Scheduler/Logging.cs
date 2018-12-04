using System;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Context;
using Serilog.Core;

namespace Scheduler
{
    public static class Logging
    {
        private static readonly string ApplicationName = Environment.GetEnvironmentVariable("BRIGHTHR_APPLICATION_NAME");
        private static readonly string Env = Environment.GetEnvironmentVariable("BRIGHTHR_ENVIRONMENT");

        private static readonly Logger Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(new ConfigurationBuilder().AddEnvironmentVariables().Build())
            .Enrich.FromLogContext()
            .CreateLogger();

        public static void Success(string message, string functionName)
        {
            using (LogContext.PushProperty("tags", new[] { ApplicationName, Env }))
            {
                Logger.Information(
                    "{Reason}. FunctionName={FunctionName}",
                    message,
                    functionName);
            }
        }

        public static void Error(string message, string functionName, Exception exception)
        {
            using (LogContext.PushProperty("tags", new[] { ApplicationName, Env }))
            {
                Logger.Error(
                    exception,
                    "{Reason}. FunctionName={FunctionName}",
                    message,
                    functionName);
            }
        }
    }
}

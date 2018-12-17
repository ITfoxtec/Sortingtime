using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Sortingtime.PdfMailWebJob.Infrastructure.Logging;
using System;
using System.Configuration;
using System.Diagnostics;

namespace Sortingtime.PdfMailWebJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["ApplicationInsights:InstrumentationKey"];
#if DEBUG
            TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode = true;
#endif

            var config = new JobHostConfiguration();
            config.Queues.MaxPollingInterval = TimeSpan.FromSeconds(2);
            config.Tracing.Tracers.Add(new WebJobTraceWriter(TraceLevel.Info));

            JobHost host = new JobHost(config);
            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
        }
    }
}

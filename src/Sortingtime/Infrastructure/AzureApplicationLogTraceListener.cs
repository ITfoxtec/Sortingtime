using Microsoft.ApplicationInsights;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Extensions.Configuration;

namespace Sortingtime.Infrastructure
{
    //http://stackoverflow.com/questions/34866674/logging-from-asp-net-5-application-hosted-as-azure-web-app
    public class AzureApplicationLogTraceListener : TraceListener
    {
        //private readonly string _logPath;
        //private readonly object _lock = new object();
        
        public AzureApplicationLogTraceListener()
        {

            //string instanceId = Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID");
            //if (instanceId != null)
            //{
            //    string logFolder = Environment.ExpandEnvironmentVariables(@"%HOME%\LogFiles\application");
            //    Directory.CreateDirectory(logFolder);
            //    instanceId = instanceId.Substring(0, 6);
            //    _logPath = Path.Combine(logFolder, $"logs_{instanceId}.txt");

            //}
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            WriteLine(data.ToString());
        }

        public override void Write(string message)
        {
            //var ai = new TelemetryClient();
            //var trace = new TraceTelemetry("test1 " + message);
            //trace.SeverityLevel = SeverityLevel.Critical;
            //ai.Track(trace);

            //try
            //{
            //    throw new Exception("error3" + message);
            //}
            //catch (Exception exc)
            //{
            //    var telemetry = new Microsoft.ApplicationInsights.TelemetryClient();
            //    telemetry.TrackException(exc);
            //}



            // Ignoreres...


            //    //if (_logPath != null)
            //    //{
            //    //    lock (this)
            //    //    {
            //    //        File.AppendAllText(_logPath, $"{DateTime.UtcNow.ToShortDateString()}-{DateTime.UtcNow.ToShortTimeString()} : {message}");
            //    //    }
            //    //}
        }

        public override void WriteLine(string message)
        {
            //// Note: A single instance of telemetry client is sufficient to track multiple telemetry items.
            //var ai = new TelemetryClient();
            //ai.TrackException(filterContext.Exception);
            //var ai = new TelemetryClient();
            //var trace = new TraceTelemetry("test2 " + message);
            //trace.SeverityLevel = SeverityLevel.Critical;
            //ai.Track(trace);

            try
            {
                throw new Exception("error2" + message);
            }
            catch (Exception exc)
            {
                var telemetry = new Microsoft.ApplicationInsights.TelemetryClient();
                telemetry.TrackTrace("anders test", SeverityLevel.Error);

                telemetry.TrackTrace("Slow database response",
                    SeverityLevel.Warning,
                    new System.Collections.Generic.Dictionary<string, string> { { "database", "1111" } });
                telemetry.TrackException(exc);

            }


            //var storageAccount = CloudStorageAccount.Parse(Startup.Configuration.GetConnectionString("AzureWebJobsStorage"));
            //var blobClient = storageAccount.CreateCloudBlobClient();
            //var logBlobContainer = blobClient.GetContainerReference("websitelog");
            //logBlobContainer.CreateIfNotExists();

            //var now = DateTime.Now;
            //var logBlob = logBlobContainer.GetBlockBlobReference($"{now.Year.ToString("D4")}/{now.Month.ToString("D2")}/{now.Day.ToString("D2")} {now.Hour.ToString("D2")}:{now.Minute.ToString("D2")}:{now.Second.ToString("D2")};{now.Millisecond} ERROR");
            //logBlob.UploadText(message);
        }
    }
}

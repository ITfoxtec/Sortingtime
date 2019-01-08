//using Microsoft.ApplicationInsights;
//using Microsoft.ApplicationInsights.DataContracts;
//using Microsoft.Azure.WebJobs.Host;
//using Sortingtime.Models;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;

//namespace Sortingtime.PdfMailWebJob.Infrastructure.Logging
//{
//    public class WebJobTraceWriter : TraceWriter
//    {
//        public WebJobTraceWriter(TraceLevel level) : base(level)
//        { }

//        public override void Trace(TraceEvent traceEvent)
//        {

//            try
//            {
//                if (/*traceEvent.Level == TraceLevel.Warning ||*/ traceEvent.Level == TraceLevel.Error)
//                {
//                    var telemetryClient = new TelemetryClient();

//                    string message = traceEvent.Message;
//                    if (!string.IsNullOrEmpty(message))
//                    {
//                        if (traceEvent.Exception != null)
//                        {
//                            message = $"{message}, {traceEvent.Exception.Message}";
//                        }
//                        var properties = new Dictionary<string, string>();
//                        properties.Add("message", message);
//                        telemetryClient.TrackException(traceEvent.Exception, properties);
//                    }
//                    else
//                    {
//                        telemetryClient.TrackException(traceEvent.Exception);
//                    }

//                }
//                //else if (traceEvent.Level == TraceLevel.Verbose || traceEvent.Level == TraceLevel.Info)
//                //{
//                //    string message = traceEvent.Message;
//                //    if (!string.IsNullOrEmpty(message))
//                //    {
//                //        telemetryClient.TrackTrace(message, GetSeverityLevel(traceEvent.Level));
//                //    }
//                //}
//            }
//            catch { };

//            try
//            {
//                if (traceEvent.Level == TraceLevel.Warning || traceEvent.Level == TraceLevel.Error)
//                {
//                    string message = traceEvent.Message;
//                    if (!string.IsNullOrEmpty(message))
//                    {
//                        if (traceEvent.Exception != null)
//                        {
//                            message = $"{message}, {traceEvent.Exception.ToString()}";
//                        }

//                        DBLog(traceEvent.Level, message);
//                    }
//                    else
//                    {
//                        DBLog(traceEvent.Level, traceEvent.Exception != null ? traceEvent.Exception.ToString() : "Error without message!");
//                    }

//                }
//                else if (traceEvent.Level == TraceLevel.Verbose || traceEvent.Level == TraceLevel.Info)
//                {
//                    string message = traceEvent.Message;
//                    if (!string.IsNullOrEmpty(message))
//                    {
//                        DBLog(traceEvent.Level, message);
//                    }
//                }
//            }
//            catch { }
//        }

//        private void DBLog(TraceLevel logLevel, string message)
//        {
//            using (var dbContext = new ApplicationConfigDbContext())
//            {
//                dbContext.Logs.Add(new Log
//                {
//                    Timestamp = DateTime.UtcNow,
//                    Severity = GetLogSeverity(logLevel),
//                    LogSource = "WebJob.TraceWriter",
//                    Message = message
//                });
//                dbContext.SaveChanges();
//            }
//        }


//        private SeverityLevel GetSeverityLevel(TraceLevel logLevel)
//        {
//            switch (logLevel)
//            {
//                case TraceLevel.Error:
//                    return SeverityLevel.Critical;
//                case TraceLevel.Warning:
//                    return SeverityLevel.Warning;
//                case TraceLevel.Info:
//                    return SeverityLevel.Information;
//                case TraceLevel.Verbose:
//                default:
//                    return SeverityLevel.Verbose;
//            }
//        }

//        private LogSeverity GetLogSeverity(TraceLevel logLevel)
//        {
//            switch (logLevel)
//            {
//                case TraceLevel.Error:
//                    return LogSeverity.Critical;
//                case TraceLevel.Warning:
//                    return LogSeverity.Warning;
//                case TraceLevel.Info:
//                    return LogSeverity.Information;
//                case TraceLevel.Verbose:
//                default:
//                    return LogSeverity.Verbose;
//            }
//        }
//    }
//}

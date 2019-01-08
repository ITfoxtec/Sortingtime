using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sortingtime.PdfMailWebJob.Functions;
using Sortingtime.Infrastructure;
using System;
using System.IO;
using Sortingtime.Models;
using Sortingtime.PdfMailWebJob.Model;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Sortingtime.PdfMailWebJob
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new HostBuilder();
            if (IsDevelopment(environment))
            {
                builder.UseEnvironment(environment);
            }
            builder.ConfigureWebJobs(b =>
                {
                    b.AddAzureStorageCoreServices()
                        // This is for QueueTrigger support
                        .AddAzureStorage(options =>
                        {
                            //options.BatchSize = 1;
                            options.MaxPollingInterval = TimeSpan.FromSeconds(2);
                            //options.VisibilityTimeout = TimeSpan.FromSeconds(30);
                        });
                    // This is for TimerTrigger support
                    //b.AddTimers();
                })
                .ConfigureAppConfiguration(b =>
                {
                    b.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false);
                    if (IsDevelopment(environment))
                    {
                        b.AddJsonFile($"appsettings.{environment}.json", optional: true);
                    }
                    b.AddEnvironmentVariables();
                })
                .ConfigureLogging((context, b) =>
                {
                    b.SetMinimumLevel(LogLevel.Trace);
                    if (IsDevelopment(environment))
                    {
                        b.AddConsole();
                    }

                    string appInsightsKey = context.Configuration["ApplicationInsights:InstrumentationKey"];
                    if (!string.IsNullOrEmpty(appInsightsKey))
                    {
                        b.AddApplicationInsights(o => o.InstrumentationKey = appInsightsKey);
                        if (IsDevelopment(environment))
                        {
                            TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode = true;
                        }
                    }
                   
                })
                .UseConsoleLifetime()     
                .ConfigureServices((context, services) =>
                 {
                     services.BindConfig<MailSettings>(context.Configuration, "Mail");
                     services.AddTransient<ApplicationDbContext>(b => new ApplicationConfigDbContext(context.Configuration.GetConnectionString("DefaultConnection")));
                     services.AddTransient<EmailMessageProvider>();
                     services.AddTransient<ReportFunction>();
                     services.AddTransient<InvoiceFunction>();
                 });

            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
        }
        
        private static bool IsDevelopment(string environment)
        {
            return "Development".Equals(environment, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}

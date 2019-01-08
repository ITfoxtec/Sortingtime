using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Sortingtime.Infrastructure;
using Sortingtime.Models;

namespace Sortingtime.Infrastructure.Configuration
{
    public static class SortingtimeServiceCollectionExtensions
    {
        public static IServiceCollection AddSortingtime(this IServiceCollection services, IConfiguration configuration)
        {
            services.BindConfig<MailSettings>(configuration, "Mail");
            
            services.AddSortingtimeLogic();
            services.AddSortingtimeProviders();

            return services;
        }

        public static IServiceCollection AddSortingtimeDataProtection(this IServiceCollection services, IConfiguration configuration)
        {
            var storageAccount = CloudStorageAccount.Parse(configuration.GetConnectionString("AzureWebJobsStorage"));
            var cloudBlobClient = storageAccount.CreateCloudBlobClient();
            var dataProtectionBlob = cloudBlobClient.GetContainerReference("dataprotection");
            dataProtectionBlob.CreateIfNotExists();

            services.AddDataProtection().PersistKeysToAzureBlobStorage(dataProtectionBlob, "keys.xml");

            return services;
        }      

        private static IServiceCollection AddSortingtimeLogic(this IServiceCollection services)
        {
            //services.AddTransient<GenerateReport>();

            return services;
        }

        private static IServiceCollection AddSortingtimeProviders(this IServiceCollection services)
        {
            services.AddTransient<EmailMessageProvider>();

            return services;
        }
    }
}

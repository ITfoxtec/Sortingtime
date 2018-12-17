using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;

namespace Sortingtime.Infrastructure.Configuration
{
    public static class SortingtimeServiceCollectionExtensions
    {
        public static IServiceCollection AddSortingtime(this IServiceCollection services)
        {
            services.AddSortingtimeLogic();
            services.AddSortingtimeProviders();

            return services;
        }

        public static IServiceCollection AddSortingtimeAddDataProtection(this IServiceCollection services, IConfiguration configuration)
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

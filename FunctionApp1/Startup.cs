using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(FunctionApp1.Startup))]
namespace FunctionApp1
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(
            IFunctionsHostBuilder builder)
        {
            var services =
                builder.Services;

            var toDoEntityDataStoreOptions =
                new ToDoEntityDataStoreOptions();

            var primaryCloudStorageAccount =
                CloudStorageAccount.Parse(
                    Environment.GetEnvironmentVariable("AzureTableStorageOptions:PrimaryConnectionString"));

            toDoEntityDataStoreOptions.PrimaryCloudTableClient =
                primaryCloudStorageAccount.CreateCloudTableClient();

            if (!string.IsNullOrWhiteSpace(
                Environment.GetEnvironmentVariable("AzureTableStorageOptions:SecondaryConnectionString")))
            {
                var secondaryCloudStorageAccount =
                    CloudStorageAccount.Parse(
                        Environment.GetEnvironmentVariable("AzureTableStorageOptions:SecondaryConnectionString"));

                toDoEntityDataStoreOptions.SecondaryCloudTableClient =
                    secondaryCloudStorageAccount.CreateCloudTableClient();
            }

            services.AddSingleton(toDoEntityDataStoreOptions);

            services.AddTransient<IToDoEntityDataStore, ToDoEntityDataStore>();
        }
    }
}

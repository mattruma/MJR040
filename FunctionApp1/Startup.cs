using ClassLibrary1;
using FunctionApp1.Data;
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

            var entityDataStoreOptions =
                new EntityDataStoreOptions();

            var primaryCloudStorageAccount =
                CloudStorageAccount.Parse(
                    Environment.GetEnvironmentVariable("AzureTableStorageOptions:PrimaryConnectionString"));

            entityDataStoreOptions.PrimaryCloudTableClient =
                primaryCloudStorageAccount.CreateCloudTableClient();

            if (!string.IsNullOrWhiteSpace(
                Environment.GetEnvironmentVariable("AzureTableStorageOptions:SecondaryConnectionString")))
            {
                var secondaryCloudStorageAccount =
                    CloudStorageAccount.Parse(
                        Environment.GetEnvironmentVariable("AzureTableStorageOptions:SecondaryConnectionString"));

                entityDataStoreOptions.SecondaryCloudTableClient =
                    secondaryCloudStorageAccount.CreateCloudTableClient();
            }

            services.AddSingleton(entityDataStoreOptions);

            services.AddTransient<IToDoEntityDataStore, ToDoEntityDataStore>();
            services.AddTransient<IToDoCommentEntityDataStore, ToDoCommentEntityDataStore>();
        }
    }
}

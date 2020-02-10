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

            var primaryCloudStorageAccount =
                CloudStorageAccount.Parse(
                    Environment.GetEnvironmentVariable("AzureTableStorageOptions:PrimaryConnectionString"));

            var primaryCloudClient =
                primaryCloudStorageAccount.CreateCloudTableClient();

            services.AddTransient<IToDoEntityDataStore, ToDoEntityDataStore>();
        }
    }
}

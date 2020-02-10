using Bogus;
using FunctionApp1.Tests.Helpers;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Xunit;

namespace FunctionApp1.Tests
{
    public abstract class BaseTests : IAsyncLifetime
    {
        protected readonly IConfiguration _configuration;
        protected readonly ILogger _logger = LoggerHelper.CreateLogger(LoggerTypes.List);
        protected readonly Faker _faker;
        protected CloudTableClient _cloudTableClient;

        protected BaseTests()
        {
            // NOTE: Make sure to set these files to copy to output directory

            _configuration = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.json")
                 .AddJsonFile("appsettings.Development.json")
                 .Build();

            _faker = new Faker();

            var cloudStorageAccount =
                CloudStorageAccount.Parse(
                    _configuration["AzureTableStorageOptions:ConnectionString"]);

            _cloudTableClient =
                cloudStorageAccount.CreateCloudTableClient();
        }

        public Task DisposeAsync()
        {
            _cloudTableClient = null;

            return Task.CompletedTask;
        }

        public async Task InitializeAsync()
        {
            CloudTable cloudTable;

            cloudTable =
                _cloudTableClient.GetTableReference("todos");

            await cloudTable.CreateIfNotExistsAsync();
        }
    }
}

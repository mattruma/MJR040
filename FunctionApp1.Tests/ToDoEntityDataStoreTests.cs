using FluentAssertions;
using FunctionApp1.Tests.Helpers;
using Microsoft.Azure.Cosmos.Table;
using Moq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace FunctionApp1.Tests
{
    public class ToDoEntityDataStoreTests : BaseTests
    {
        [Fact]
        public async Task When_AddAsync()
        {
            // Arrange

            var toDoEntityStore =
                new ToDoEntityDataStore(
                    _configuration["AzureTableStorageOptions:ConnectionString"]);

            var toDoEntity =
                _faker.GenerateToDoEntity();

            // Action

            await toDoEntityStore.AddAsync(
                toDoEntity);

            // Assert

            var cloudTable =
                _cloudTableClient.GetTableReference("todos");

            var tableOperation =
                TableOperation.Retrieve<ToDoEntity>(toDoEntity.PartitionKey, toDoEntity.RowKey);

            var tableResult =
                await cloudTable.ExecuteAsync(tableOperation);

            tableResult.HttpStatusCode.Should().Be((int)HttpStatusCode.OK);

            var toDoEntityFetched =
                tableResult.Result as ToDoEntity;

            toDoEntityFetched.Should().NotBeNull();
            toDoEntityFetched.RowKey.Should().Be(toDoEntity.RowKey);
            toDoEntityFetched.PartitionKey.Should().Be(toDoEntity.PartitionKey);
            toDoEntityFetched.Status.Should().Be(toDoEntity.Status);
            toDoEntityFetched.Description.Should().Be(toDoEntity.Description);
        }

        // https://stackoverflow.com/questions/58957600/cloudtableclient-unit-testing
        // https://fluentassertions.com/exceptions/

        [Fact]
        public void When_AddAsync_And_PrimaryStorageNotAvailable_ThenThrowsHttpStatusException()
        {
            // Arrange

            var primaryCloudTable = new Mock<CloudTable>(
                new Uri("http://localhost.com/PrimaryConnectionString"), null);

            var primaryCloudTableResult =
                new TableResult
                {
                    HttpStatusCode = (int)HttpStatusCode.NotFound
                };

            primaryCloudTable.Setup(x => x.ExecuteAsync(It.IsAny<TableOperation>()))
                    .ReturnsAsync(primaryCloudTableResult);

            var toDoEntityStore =
                new ToDoEntityDataStore(
                    primaryCloudTable.Object);

            var toDoEntity =
                _faker.GenerateToDoEntity();

            // Action

            Func<Task> action = async () => await toDoEntityStore.AddAsync(toDoEntity);

            // Assert

            action.Should().Throw<HttpRequestException>();
        }

        // https://stackoverflow.com/questions/9136674/verify-a-method-call-using-moq

        [Fact]
        public async Task When_AddAsync_And_PrimaryStorageNotAvailable_ThenFailOverToSecondaryStorage()
        {
            // Arrange

            var primaryCloudTable = new Mock<CloudTable>(
                new Uri("https://localhost.com/primary"), null);

            var primaryCloudTableResult =
                new TableResult
                {
                    HttpStatusCode = (int)HttpStatusCode.TooManyRequests
                };

            primaryCloudTable.Setup(x => x.ExecuteAsync(It.IsAny<TableOperation>()))
                    .ReturnsAsync(primaryCloudTableResult);

            var secondaryCloudTable = new Mock<CloudTable>(
                new Uri("https://localhost.com/secondary"), null);

            var secondaryCloudTableResult =
                new TableResult
                {
                    HttpStatusCode = (int)HttpStatusCode.OK
                };

            secondaryCloudTable.Setup(x => x.ExecuteAsync(It.IsAny<TableOperation>()))
                    .ReturnsAsync(secondaryCloudTableResult);

            var toDoEntityStore =
                new ToDoEntityDataStore(
                    primaryCloudTable.Object,
                    secondaryCloudTable.Object);

            var toDoEntity =
                _faker.GenerateToDoEntity();

            // Action

            await toDoEntityStore.AddAsync(toDoEntity);

            // Assert

            secondaryCloudTable.Verify(x => x.ExecuteAsync(It.IsAny<TableOperation>()), Times.Once);
        }

        [Fact]
        public async Task When_GetByIdAsync()
        {
            // Arrange

            var toDoEntity =
                _faker.GenerateToDoEntity();

            var cloudTable =
                _cloudTableClient.GetTableReference("todos");

            var tableOperation =
                TableOperation.Insert(toDoEntity);

            await cloudTable.ExecuteAsync(tableOperation);

            var toDoEntityStore =
                new ToDoEntityDataStore(
                    _configuration["AzureTableStorageOptions:ConnectionString"]);

            // Action

            var toDoEntityFetched = await toDoEntityStore.GetByIdAsync(
                toDoEntity.RowKey);

            // Assert

            toDoEntityFetched.Should().NotBeNull();
            toDoEntityFetched.RowKey.Should().Be(toDoEntity.RowKey);
            toDoEntityFetched.PartitionKey.Should().Be(toDoEntity.PartitionKey);
            toDoEntityFetched.Status.Should().Be(toDoEntity.Status);
            toDoEntityFetched.Description.Should().Be(toDoEntity.Description);
        }



        [Fact]
        public void When_GetByIdAsync_And_EntityDoesNotExist_Then_ThrowException()
        {
            // Arrange

            var toDoEntityStore =
                new ToDoEntityDataStore(
                    _configuration["AzureTableStorageOptions:ConnectionString"]);

            // Action

            Func<Task> action = async () => await toDoEntityStore.GetByIdAsync(
                Guid.NewGuid().ToString());

            // Assert

            action.Should().Throw<HttpRequestException>().WithMessage("Something went wrong in table operation, a 404 status code was returned.");
        }

        [Fact]
        public async Task When_DeleteAsync()
        {
            // Arrange

            var toDoEntity =
                _faker.GenerateToDoEntity();

            var cloudTable =
                _cloudTableClient.GetTableReference("todos");

            var tableOperation =
                TableOperation.Insert(toDoEntity);

            await cloudTable.ExecuteAsync(tableOperation);

            var toDoEntityStore =
                new ToDoEntityDataStore(
                    _configuration["AzureTableStorageOptions:ConnectionString"]);

            // Action

            await toDoEntityStore.DeleteAsync(
                toDoEntity);

            // Assert

            tableOperation =
                TableOperation.Retrieve<ToDoEntity>(
                    toDoEntity.PartitionKey, toDoEntity.RowKey);

            var tableResult =
                await cloudTable.ExecuteAsync(tableOperation);

            tableResult.HttpStatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task When_DeleteByIdAsync()
        {
            // Arrange

            var toDoEntity =
                _faker.GenerateToDoEntity();

            var cloudTable =
                _cloudTableClient.GetTableReference("todos");

            var tableOperation =
                TableOperation.Insert(toDoEntity);

            await cloudTable.ExecuteAsync(tableOperation);

            var toDoEntityStore =
                new ToDoEntityDataStore(
                    _configuration["AzureTableStorageOptions:ConnectionString"]);

            // Action

            await toDoEntityStore.DeleteByIdAsync(
                toDoEntity.RowKey);

            // Assert

            tableOperation =
                TableOperation.Retrieve<ToDoEntity>(
                    toDoEntity.PartitionKey, toDoEntity.RowKey);

            var tableResult =
                await cloudTable.ExecuteAsync(tableOperation);

            tableResult.HttpStatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task When_UpdateAsync()
        {
            // Arrange

            var toDoEntity =
                _faker.GenerateToDoEntity();

            var cloudTable =
                _cloudTableClient.GetTableReference("todos");

            var tableOperation =
                TableOperation.Insert(toDoEntity);

            await cloudTable.ExecuteAsync(tableOperation);

            var toDoEntityStore =
                new ToDoEntityDataStore(
                    _configuration["AzureTableStorageOptions:ConnectionString"]);

            toDoEntity.Description = _faker.Lorem.Paragraph(1);

            // Action

            await toDoEntityStore.UpdateAsync(
                toDoEntity);

            // Assert

            tableOperation =
                TableOperation.Retrieve<ToDoEntity>(
                    toDoEntity.PartitionKey, toDoEntity.RowKey);

            var tableResult =
                await cloudTable.ExecuteAsync(tableOperation);

            var toDoEntityUpdated =
                tableResult.Result as ToDoEntity;

            toDoEntityUpdated.Should().NotBeNull();
            toDoEntityUpdated.Description.Should().Be(toDoEntity.Description);
        }

        [Fact]
        public async Task When_ListAsync()
        {
            // Arrange

            for (var i = 0; i < 3; i++)
            {
                var toDoEntity =
                    _faker.GenerateToDoEntity();

                var cloudTable =
                    _cloudTableClient.GetTableReference("todos");

                var tableOperation =
                    TableOperation.Insert(toDoEntity);

                await cloudTable.ExecuteAsync(tableOperation);
            }

            var toDoEntityStore =
                new ToDoEntityDataStore(
                    _configuration["AzureTableStorageOptions:ConnectionString"]);

            // Action

            var toDoEntityFetchedList =
                await toDoEntityStore.ListAsync();

            // Assert

            toDoEntityFetchedList.Should().NotBeNull();
            toDoEntityFetchedList.Count().Should().BeGreaterOrEqualTo(3);
        }
    }
}

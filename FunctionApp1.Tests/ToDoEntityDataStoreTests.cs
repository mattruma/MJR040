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

            var toDoEntityDataStoreOptions =
                new ToDoEntityDataStoreOptions
                {
                    PrimaryCloudTableClient = _cloudTableClient
                };

            var toDoEntityDataStore =
                new ToDoEntityDataStore(
                    toDoEntityDataStoreOptions);

            var toDoEntity =
                _faker.GenerateToDoEntity();

            // Action

            await toDoEntityDataStore.AddAsync(
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

            var primaryCloudTableClient =
                new Mock<CloudTableClient>(
                    new Uri("http://localhost.com"), null, null);

            var primaryCloudTable = new Mock<CloudTable>(
                new Uri("http://localhost.com"), null);

            var primaryCloudTableResult =
                new TableResult
                {
                    HttpStatusCode = (int)HttpStatusCode.NotFound
                };

            primaryCloudTable.Setup(x => x.ExecuteAsync(It.IsAny<TableOperation>()))
                    .ReturnsAsync(primaryCloudTableResult);

            primaryCloudTableClient.Setup(x => x.GetTableReference("todos"))
                    .Returns(primaryCloudTable.Object);

            var toDoEntityDataStoreOptions =
                new ToDoEntityDataStoreOptions
                {
                    PrimaryCloudTableClient = primaryCloudTableClient.Object
                };

            var toDoEntityDataStore =
                new ToDoEntityDataStore(
                    toDoEntityDataStoreOptions);

            var toDoEntity =
                _faker.GenerateToDoEntity();

            // Action

            Func<Task> action = async () => await toDoEntityDataStore.AddAsync(toDoEntity);

            // Assert

            action.Should().Throw<HttpRequestException>();
        }

        // https://stackoverflow.com/questions/9136674/verify-a-method-call-using-moq

        [Fact]
        public async Task When_AddAsync_And_PrimaryStorageNotAvailable_ThenFailOverToSecondaryStorage()
        {
            // Arrange

            var primaryCloudTableClient =
                new Mock<CloudTableClient>(
                    new Uri("http://localhost.com"), null, null);

            var primaryCloudTable = new Mock<CloudTable>(
                new Uri("https://localhost.com"), null);

            var primaryCloudTableResult =
                new TableResult
                {
                    HttpStatusCode = (int)HttpStatusCode.TooManyRequests
                };

            primaryCloudTable.Setup(x => x.ExecuteAsync(It.IsAny<TableOperation>()))
                    .ReturnsAsync(primaryCloudTableResult);

            primaryCloudTableClient.Setup(x => x.GetTableReference("todos"))
                    .Returns(primaryCloudTable.Object);

            var secondaryCloudTableClient =
                new Mock<CloudTableClient>(
                    new Uri("http://localhost.com"), null, null);

            var secondaryCloudTable = new Mock<CloudTable>(
                new Uri("https://localhost.com/secondary"), null);

            var secondaryCloudTableResult =
                new TableResult
                {
                    HttpStatusCode = (int)HttpStatusCode.OK
                };

            secondaryCloudTable.Setup(x => x.ExecuteAsync(It.IsAny<TableOperation>()))
                    .ReturnsAsync(secondaryCloudTableResult);

            secondaryCloudTableClient.Setup(x => x.GetTableReference("todos"))
                    .Returns(secondaryCloudTable.Object);

            var toDoEntityDataStoreOptions =
                new ToDoEntityDataStoreOptions
                {
                    PrimaryCloudTableClient = primaryCloudTableClient.Object,
                    SecondaryCloudTableClient = secondaryCloudTableClient.Object
                };

            var toDoEntityDataStore =
                new ToDoEntityDataStore(
                    toDoEntityDataStoreOptions);

            var toDoEntity =
                _faker.GenerateToDoEntity();

            // Action

            await toDoEntityDataStore.AddAsync(toDoEntity);

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

            var toDoEntityDataStoreOptions =
                new ToDoEntityDataStoreOptions
                {
                    PrimaryCloudTableClient = _cloudTableClient
                };

            var toDoEntityDataStore =
                new ToDoEntityDataStore(
                    toDoEntityDataStoreOptions);

            // Action

            var toDoEntityFetched = await toDoEntityDataStore.GetByIdAsync(
                toDoEntity.RowKey);

            // Assert

            toDoEntityFetched.Should().NotBeNull();
            toDoEntityFetched.RowKey.Should().Be(toDoEntity.RowKey);
            toDoEntityFetched.PartitionKey.Should().Be(toDoEntity.PartitionKey);
            toDoEntityFetched.Status.Should().Be(toDoEntity.Status);
            toDoEntityFetched.Description.Should().Be(toDoEntity.Description);
        }



        [Fact]
        public async Task When_GetByIdAsync_And_EntityDoesNotExist_Then_ReturnNull()
        {
            // Arrange

            var toDoEntityDataStoreOptions =
                new ToDoEntityDataStoreOptions
                {
                    PrimaryCloudTableClient = _cloudTableClient
                };

            var toDoEntityDataStore =
                new ToDoEntityDataStore(
                    toDoEntityDataStoreOptions);

            // Action

            var toDoEntity =
                await toDoEntityDataStore.GetByIdAsync(
                    Guid.NewGuid().ToString());

            // Assert

            toDoEntity.Should().BeNull();
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

            var toDoEntityDataStoreOptions =
                new ToDoEntityDataStoreOptions
                {
                    PrimaryCloudTableClient = _cloudTableClient
                };

            var toDoEntityDataStore =
                new ToDoEntityDataStore(
                    toDoEntityDataStoreOptions);

            // Action

            await toDoEntityDataStore.DeleteAsync(
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

            var toDoEntityDataStoreOptions =
                new ToDoEntityDataStoreOptions
                {
                    PrimaryCloudTableClient = _cloudTableClient
                };

            var toDoEntityDataStore =
                new ToDoEntityDataStore(
                    toDoEntityDataStoreOptions);

            // Action

            await toDoEntityDataStore.DeleteByIdAsync(
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

            var toDoEntityDataStoreOptions =
                new ToDoEntityDataStoreOptions
                {
                    PrimaryCloudTableClient = _cloudTableClient
                };

            var toDoEntityDataStore =
                new ToDoEntityDataStore(
                    toDoEntityDataStoreOptions);

            toDoEntity.Description = _faker.Lorem.Paragraph(1);

            // Action

            await toDoEntityDataStore.UpdateAsync(
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

            var toDoEntityDataStoreOptions =
                new ToDoEntityDataStoreOptions
                {
                    PrimaryCloudTableClient = _cloudTableClient
                };

            var toDoEntityDataStore =
                new ToDoEntityDataStore(
                    toDoEntityDataStoreOptions);

            // Action

            var toDoEntityFetchedList =
                await toDoEntityDataStore.ListAsync();

            // Assert

            toDoEntityFetchedList.Should().NotBeNull();
            toDoEntityFetchedList.Count().Should().BeGreaterOrEqualTo(3);
        }
    }
}

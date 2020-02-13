using ClassLibrary2;
using FluentAssertions;
using FunctionApp1.Data;
using FunctionApp1.Tests.Helpers;
using Microsoft.Azure.Cosmos.Table;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace FunctionApp1.Tests
{
    public class ToDoCommentEntityDataStoreTests : BaseTests
    {
        [Fact]
        public async Task When_AddAsync()
        {
            // Arrange

            var toDoEntity =
                _faker.GenerateToDoEntity();

            var cloudTable =
                _cloudTableClient.GetTableReference("todos");

            var tableOperation =
                TableOperation.Insert(toDoEntity);

            await cloudTable.ExecuteAsync(tableOperation);

            var entityDataStoreOptions =
                new EntityDataStoreOptions
                {
                    PrimaryCloudTableClient = _cloudTableClient
                };

            var toDoCommentEntityDataStore =
                new ToDoCommentEntityDataStore(
                    entityDataStoreOptions);

            var toDoCommentEntity =
                _faker.GenerateToDoCommentEntity(toDoEntity.Id);

            // Action

            await toDoCommentEntityDataStore.AddAsync(
                toDoEntity.Id,
                toDoCommentEntity);

            // Assert

            cloudTable =
                _cloudTableClient.GetTableReference("todos");

            tableOperation =
                TableOperation.Retrieve<ToDoCommentEntity>(toDoCommentEntity.PartitionKey, toDoCommentEntity.RowKey);

            var tableResult =
                await cloudTable.ExecuteAsync(tableOperation);

            tableResult.HttpStatusCode.Should().Be((int)HttpStatusCode.OK);

            var toDoCommentEntityFetched =
                tableResult.Result as ToDoCommentEntity;

            toDoCommentEntityFetched.Should().NotBeNull();
            toDoCommentEntityFetched.RowKey.Should().Be(toDoCommentEntity.RowKey);
            toDoCommentEntityFetched.PartitionKey.Should().Be(toDoCommentEntity.PartitionKey);
            toDoCommentEntityFetched.Body.Should().Be(toDoCommentEntity.Body);
        }

        [Fact]
        public async Task When_ListAsync()
        {
            // Arrange

            var cloudTable =
                _cloudTableClient.GetTableReference("todos");

            var toDoEntity =
                _faker.GenerateToDoEntity();

            var tableOperation =
                TableOperation.Insert(toDoEntity);

            await cloudTable.ExecuteAsync(tableOperation);

            for (var i = 0; i < 3; i++)
            {
                var toDoCommentEntity =
                    _faker.GenerateToDoCommentEntity(
                        toDoEntity.Id);

                tableOperation =
                    TableOperation.Insert(toDoCommentEntity);

                await cloudTable.ExecuteAsync(tableOperation);
            }

            var entityDataStoreOptions =
                new EntityDataStoreOptions
                {
                    PrimaryCloudTableClient = _cloudTableClient
                };

            var toDoCommentEntityDataStore =
                new ToDoCommentEntityDataStore(
                    entityDataStoreOptions);

            // Action

            var toDoCommentEntityList =
                await toDoCommentEntityDataStore.ListByToDoIdAsync(
                    toDoEntity.Id);

            // Assert

            toDoCommentEntityList.Should().NotBeNull();
            toDoCommentEntityList.Count().Should().Be(3);
        }
    }
}

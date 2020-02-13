using Bogus;
using FunctionApp1.Data;
using System;

namespace FunctionApp1.Tests.Helpers
{
    public static class FakerExtensions
    {
        public static ToDoEntity GenerateToDoEntity(
            this Faker faker)
        {
            var id =
                Guid.NewGuid().ToString();

            var toDoEntity =
                new ToDoEntity
                {
                    Id = id,
                    RowKey = id,
                    PartitionKey = id,
                    Status = faker.Random.ArrayElement(new[] { "Pending", "In Progress", "Completed", "Canceled" }),
                    Description = faker.Lorem.Paragraph(1)
                };

            return toDoEntity;
        }

        public static ToDoCommentEntity GenerateToDoCommentEntity(
           this Faker faker,
           string toDoId)
        {
            var id =
                Guid.NewGuid().ToString();

            var toDoCommentEntity =
                new ToDoCommentEntity
                {
                    Id = id,
                    RowKey = id,
                    PartitionKey = toDoId,
                    Body = faker.Lorem.Paragraph(1),
                    CreatedOn = faker.Date.Recent()
                };

            return toDoCommentEntity;
        }
    }
}

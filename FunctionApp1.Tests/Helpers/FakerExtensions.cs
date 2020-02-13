using Bogus;

namespace FunctionApp1.Tests.Helpers
{
    public static class FakerExtensions
    {
        public static ToDoEntity GenerateToDoEntity(
            this Faker faker)
        {
            var toDoEntity =
                new ToDoEntity
                {
                    Status = faker.Random.ArrayElement(new[] { "Pending", "In Progress", "Completed", "Canceled" }),
                    Description = faker.Lorem.Paragraph(1)
                };

            return toDoEntity;
        }
        public static ToDoCommentEntity GenerateToDoCommentEntity(
           this Faker faker,
           string parentId)
        {
            var toDoCommentEntity =
                new ToDoCommentEntity(parentId)
                {
                    Body = faker.Lorem.Paragraph(1),
                    CreatedOn = faker.Date.Recent()
                };

            return toDoCommentEntity;
        }
    }
}

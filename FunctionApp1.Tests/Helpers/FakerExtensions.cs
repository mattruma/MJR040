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
    }
}

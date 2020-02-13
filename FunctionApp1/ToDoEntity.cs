using ClassLibrary2;
using System;

namespace FunctionApp1
{
    public class ToDoEntity : Entity<string>
    {
        public string Status { get; set; }
        public string Description { get; set; }

        public ToDoEntity()
        {
            this.Id = Guid.NewGuid().ToString();
        }
    }
}

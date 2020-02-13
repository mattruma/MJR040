using ClassLibrary2;
using System;

namespace FunctionApp1
{
    public class ToDoCommentEntity : ChildEntity<string>
    {
        public string ToDoId { get; set; }
        public string Body { get; set; }

        public ToDoCommentEntity()
        {
        }

        public ToDoCommentEntity(
            string toDoId)
        {
            this.Id = Guid.NewGuid().ToString();
            this.ToDoId = toDoId;
        }
    }
}

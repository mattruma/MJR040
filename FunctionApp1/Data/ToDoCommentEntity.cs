using ClassLibrary2;
using Newtonsoft.Json;
using System;

namespace FunctionApp1.Data
{
    public class ToDoCommentEntity : ChildEntity<string>
    {
        [JsonProperty("object")]
        public override string Object => "Comment";

        [JsonProperty("body")]
        public string Body { get; set; }


        public ToDoCommentEntity()
        {
            this.Id = Guid.NewGuid().ToString();
        }
    }
}

﻿using ClassLibrary2;
using Newtonsoft.Json;
using System;

namespace FunctionApp1.Data
{
    public class ToDoEntity : Entity<string>
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        public ToDoEntity()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Object = "ToDo";
        }
    }
}

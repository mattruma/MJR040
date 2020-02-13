using ClassLibrary1;
using Microsoft.Azure.Cosmos.Table;
using System;

namespace FunctionApp1
{
    public class ToDoEntity : TableEntity, IEntity
    {
        public string Status { get; set; }
        public string Description { get; set; }

        public ToDoEntity()
        {
            this.RowKey = Guid.NewGuid().ToString();
            this.PartitionKey = this.RowKey;
        }
    }
}

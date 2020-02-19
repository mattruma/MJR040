﻿using ClassLibrary2;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FunctionApp1.Data
{
    public class ToDoCommentEntityDataStore : ChildEntityDataStore<string, string, ToDoCommentEntity>, IToDoCommentEntityDataStore
    {
        public ToDoCommentEntityDataStore(
            EntityDataStoreOptions entityDataStoreOptions) : base("todos", entityDataStoreOptions)
        {
        }

        public async Task<IEnumerable<ToDoCommentEntity>> ListByToDoIdAsync(
            string toDoId)
        {
            var query =
                $"PartitionKey eq '{toDoId}' and Object eq 'Comment'";

            return await base.ListAsync(query);
        }
    }
}

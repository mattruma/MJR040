﻿using ClassLibrary2;

namespace FunctionApp1
{
    public class ToDoEntityDataStore : EntityDataStore<string, ToDoEntity>, IToDoEntityDataStore
    {
        public ToDoEntityDataStore(
            EntityDataStoreOptions entityDataStoreOptions) : base("todos", entityDataStoreOptions)
        {
        }
    }
}

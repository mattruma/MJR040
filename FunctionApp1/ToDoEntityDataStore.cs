using ClassLibrary2;

namespace FunctionApp1
{
    public class ToDoEntityDataStore : EntityDataStore<string, ToDoEntity>, IToDoEntityDataStore
    {
        public ToDoEntityDataStore(
            ToDoEntityDataStoreOptions toDoEntityDataStoreOptions) : base("todos", toDoEntityDataStoreOptions)
        {
        }
    }
}

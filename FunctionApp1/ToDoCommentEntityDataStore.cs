using ClassLibrary2;

namespace FunctionApp1
{
    public class ToDoCommentEntityDataStore : ChildEntityDataStore<string, string, ToDoCommentEntity>, IToDoCommentEntityDataStore
    {
        public ToDoCommentEntityDataStore(
            EntityDataStoreOptions entityDataStoreOptions) : base("todos", entityDataStoreOptions)
        {
        }
    }
}

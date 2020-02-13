using ClassLibrary2;
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
                $"PartionKey eq '{toDoId}' Object eq 'ToDoComment'";

            return await base.ListAsync(query);
        }
    }
}

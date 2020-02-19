using ClassLibrary2;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FunctionApp1.Data
{
    public class ToDoEntityDataStore : EntityDataStore<string, ToDoEntity>, IToDoEntityDataStore
    {
        public ToDoEntityDataStore(
            EntityDataStoreOptions entityDataStoreOptions) : base("todos", entityDataStoreOptions)
        {
        }

        public async Task<IEnumerable<ToDoEntity>> ListAsync()
        {
            var query =
                "Object eq 'ToDo'";

            return await base.ListAsync(query);
        }
    }
}

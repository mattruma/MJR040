using ClassLibrary1;

namespace FunctionApp1
{
    public interface IToDoEntityDataStore : IEntityDataStore<string, ToDoEntity>
    {
    }
}

using ClassLibrary1;

namespace FunctionApp1
{
    public interface IToDoCommentEntityDataStore : IChildEntityDataStore<string, string, ToDoCommentEntity>
    {
    }
}

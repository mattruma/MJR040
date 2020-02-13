using ClassLibrary1;
using System;

namespace FunctionApp1
{
    public interface IToDoEntityDataStore : IEntityDataStore<string, ToDoEntity>
    {
    }
}

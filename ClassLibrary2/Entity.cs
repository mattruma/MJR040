using ClassLibrary1;
using Microsoft.Azure.Cosmos.Table;

namespace ClassLibrary2
{
    public class Entity : TableEntity, IEntity
    {
        public Entity()
        {
        }
    }
}

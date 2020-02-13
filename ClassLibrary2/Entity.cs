using ClassLibrary1;
using Microsoft.Azure.Cosmos.Table;

namespace ClassLibrary2
{
    public abstract class Entity<TKey> : TableEntity, IEntity<TKey>
    {
        public TKey Id { get; set; }

        public Entity()
        {
        }
    }
}

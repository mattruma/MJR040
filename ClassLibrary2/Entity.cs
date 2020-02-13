using ClassLibrary1;
using Microsoft.Azure.Cosmos.Table;
using System;

namespace ClassLibrary2
{
    public abstract class Entity<TKey> : TableEntity, IEntity<TKey>
    {
        public TKey Id { get; set; }

        public DateTime CreatedOn { get; set; }

        protected Entity()
        {
            this.CreatedOn = DateTime.UtcNow;
        }
    }
}

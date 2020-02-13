using ClassLibrary1;
using Microsoft.Azure.Cosmos.Table;

namespace ClassLibrary2
{
    public abstract class ChildEntity<TParentKey, TKey> : TableEntity, IChildEntity<TParentKey, TKey>
    {
        public TParentKey ParentId { get; set; }

        public TKey Id { get; set; }

        public ChildEntity()
        {
        }
    }
}

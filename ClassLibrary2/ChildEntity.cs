using ClassLibrary1;

namespace ClassLibrary2
{
    public abstract class ChildEntity<TKey> : Entity<TKey>, IChildEntity<TKey>
    {
    }
}

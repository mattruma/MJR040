using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public interface IChildEntityDataStore<TParentKey, TKey, TEntity> where TEntity : IChildEntity<TKey>
    {
        Task AddAsync(
            TParentKey parentId,
            TEntity entity);
        Task DeleteByIdAsync(
            TParentKey parentId,
            TKey id);
        Task<TEntity> GetByIdAsync(
            TParentKey parentId,
            TKey id);
        Task<IEnumerable<TEntity>> ListAsync(
            TParentKey parentId);
        Task UpdateAsync(
            TParentKey parentId,
            TEntity entity);
    }
}
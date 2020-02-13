using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public interface IChildEntityDataStore<TParentKey, TKey, TEntity> where TEntity : IChildEntity<TParentKey, TKey>
    {
        Task AddAsync(
            TEntity entity);
        Task DeleteAsync(
            TEntity entity);
        Task DeleteByIdAsync(
            TParentKey parentId,
            TKey id);
        Task<TEntity> GetByIdAsync(
            TParentKey parentId,
            TKey id);
        Task<IEnumerable<TEntity>> ListAsync(
            string query = null);
        Task UpdateAsync(
            TEntity entity);
    }
}
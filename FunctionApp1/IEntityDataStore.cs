using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FunctionApp1
{
    public interface IEntityDataStore<TKey, TEntity> where TEntity : TableEntity
    {
        Task AddAsync(
            TEntity entity);
        Task DeleteAsync(
            TEntity entity);
        Task DeleteByIdAsync(
            TKey id);
        Task<TEntity> GetByIdAsync(
            TKey id);
        Task<IEnumerable<TEntity>> ListAsync(
            string query = null);
        Task UpdateAsync(
            TEntity entity);
    }
}
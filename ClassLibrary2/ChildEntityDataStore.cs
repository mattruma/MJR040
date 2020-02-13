using ClassLibrary1;
using ClassLibrary2.Helpers;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ClassLibrary2
{
    // https://www.c-sharpcorner.com/article/azure-storage-crud-operations-in-mvc-using-c-sharp-azure-table-storage-part-one/
    // http://www.mattruma.com/adventures-with-azure-table-storage-default-retry-policy/

    public abstract class ChildEntityDataStore<TParentKey, TKey, TEntity> : IChildEntityDataStore<TParentKey, TKey, TEntity> where TEntity : ChildEntity<TKey>, new()
    {
        protected readonly CloudTable _primaryCloudTable;
        protected readonly CloudTable _secondaryCloudTable;

        protected bool AutoFailover => _secondaryCloudTable != null;

        protected ChildEntityDataStore(
            string tableName,
            EntityDataStoreOptions entityDataStoreOptions)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (entityDataStoreOptions == null)
            {
                throw new ArgumentNullException(nameof(entityDataStoreOptions));
            }

            _primaryCloudTable =
                entityDataStoreOptions.PrimaryCloudTableClient.GetTableReference(tableName);

            _primaryCloudTable.CreateIfNotExists();

            if (entityDataStoreOptions.SecondaryCloudTableClient != null)
            {
                _secondaryCloudTable =
                    entityDataStoreOptions.SecondaryCloudTableClient.GetTableReference(tableName);

                _secondaryCloudTable.CreateIfNotExists();
            }
        }

        public virtual async Task AddAsync(
            TParentKey parentId,
            TEntity entity)
        {
            entity.RowKey = entity.Id.ToString();
            entity.PartitionKey = parentId.ToString();

            try
            {
                await this.AddAsync(entity, _primaryCloudTable);
            }
            catch
            {
                if (this.AutoFailover)
                {
                    await this.AddAsync(entity, _secondaryCloudTable);
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task AddAsync(
            TEntity entity,
            CloudTable cloudTable)
        {
            var tableOperation =
                TableOperation.Insert(entity);

            var tableResult =
                await cloudTable.ExecuteAsync(tableOperation);

            tableResult.EnsureSuccessStatusCode();
        }

        private async Task DeleteAsync(
            TEntity entity)
        {
            try
            {
                await this.DeleteAsync(entity, _primaryCloudTable);
            }
            catch
            {
                if (this.AutoFailover)
                {
                    await this.DeleteAsync(entity, _secondaryCloudTable);
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task DeleteAsync(
            TEntity entity,
            CloudTable cloudTable)
        {
            var tableOperation =
                TableOperation.Delete(entity);

            var tableResult =
                await cloudTable.ExecuteAsync(tableOperation);

            tableResult.EnsureSuccessStatusCode();
        }

        public virtual async Task DeleteByIdAsync(
            TParentKey parentId,
            TKey id)
        {
            var entity =
                await this.GetByIdAsync(parentId, id);

            if (entity == null) return;

            await this.DeleteAsync(entity);
        }

        public virtual async Task<TEntity> GetByIdAsync(
            TParentKey parentId,
            TKey id)
        {
            try
            {
                var entity =
                    await this.GetByIdAsync(parentId, id, _primaryCloudTable);

                return entity;
            }
            catch
            {
                if (this.AutoFailover)
                {
                    var entity =
                        await this.GetByIdAsync(parentId, id, _secondaryCloudTable);

                    return entity;
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task<TEntity> GetByIdAsync(
            TParentKey parentId,
            TKey id,
            CloudTable cloudTable)
        {
            var tableOperation =
                TableOperation.Retrieve<TEntity>(parentId.ToString(), id.ToString());

            var tableResult =
                await cloudTable.ExecuteAsync(tableOperation);

            if (tableResult.HttpStatusCode == (int)HttpStatusCode.NotFound)
            {
                return null;
            }

            tableResult.EnsureSuccessStatusCode();

            return tableResult.Result as TEntity;
        }

        public virtual async Task UpdateAsync(
            TParentKey parentId,
            TEntity entity)
        {
            entity.RowKey = entity.Id.ToString();
            entity.PartitionKey = parentId.ToString();

            try
            {
                await this.UpdateAsync(entity, _primaryCloudTable);
            }
            catch
            {
                if (this.AutoFailover)
                {
                    await this.UpdateAsync(entity, _secondaryCloudTable);
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task UpdateAsync(
            TEntity entity,
            CloudTable cloudTable)
        {
            var tableOperation =
                TableOperation.InsertOrReplace(entity);

            var tableResult =
                await cloudTable.ExecuteAsync(tableOperation);

            tableResult.EnsureSuccessStatusCode();
        }

        public virtual async Task<IEnumerable<TEntity>> ListAsync(
            TParentKey parentId)
        {
            var entityList =
                _primaryCloudTable.CreateQuery<TEntity>()
                    .Where(x => x.PartitionKey == parentId.ToString())
                    .ToList();

            return entityList;
        }
    }
}

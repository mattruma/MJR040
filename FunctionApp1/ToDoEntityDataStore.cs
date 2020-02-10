using FunctionApp1.Helpers;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FunctionApp1
{
    // https://www.c-sharpcorner.com/article/azure-storage-crud-operations-in-mvc-using-c-sharp-azure-table-storage-part-one/
    // http://www.mattruma.com/adventures-with-azure-table-storage-default-retry-policy/

    public class ToDoEntityDataStore : IToDoEntityDataStore
    {
        private readonly CloudTable _primaryCloudTable;
        private readonly CloudTable _secondaryCloudTable;

        public ToDoEntityDataStore(
            CloudTable primaryCloudTable,
            CloudTable secondaryCloudTable = null)
        {
            _primaryCloudTable = primaryCloudTable;
            _secondaryCloudTable = secondaryCloudTable;
        }

        public ToDoEntityDataStore(
            string primaryConnectionString,
            string secondaryConnectionString = null)
        {
            if (string.IsNullOrWhiteSpace(primaryConnectionString))
            {
                throw new ArgumentNullException(nameof(primaryConnectionString));
            }

            var cloudStorageAccount =
                CloudStorageAccount.Parse(primaryConnectionString);

            var cloudTableClient =
                cloudStorageAccount.CreateCloudTableClient();

            _primaryCloudTable =
                cloudTableClient.GetTableReference("todos");

            _primaryCloudTable.CreateIfNotExists();

            // Create the secondary cloud table provider if a secondary connection string has been provided

            if (!string.IsNullOrWhiteSpace(secondaryConnectionString))
            {
                cloudStorageAccount =
                    CloudStorageAccount.Parse(secondaryConnectionString);

                cloudTableClient =
                    cloudStorageAccount.CreateCloudTableClient();

                _secondaryCloudTable =
                    cloudTableClient.GetTableReference("todos");

                _secondaryCloudTable.CreateIfNotExists();
            }
        }

        private bool AutoFailover => _secondaryCloudTable != null;

        public async Task AddAsync(
            ToDoEntity toDoEntity)
        {
            try
            {
                await this.AddAsync(toDoEntity, _primaryCloudTable);
            }
            catch
            {
                if (this.AutoFailover)
                {
                    await this.AddAsync(toDoEntity, _secondaryCloudTable);
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task AddAsync(
            ToDoEntity toDoEntity,
            CloudTable cloudTable)
        {
            var tableOperation =
                TableOperation.Insert(toDoEntity);

            var tableResult =
                await cloudTable.ExecuteAsync(tableOperation);

            tableResult.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(
            ToDoEntity toDoEntity)
        {
            try
            {
                await this.DeleteAsync(toDoEntity, _primaryCloudTable);
            }
            catch
            {
                if (this.AutoFailover)
                {
                    await this.DeleteAsync(toDoEntity, _secondaryCloudTable);
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task DeleteAsync(
            ToDoEntity toDoEntity,
            CloudTable cloudTable)
        {
            var tableOperation =
                TableOperation.Delete(toDoEntity);

            var tableResult =
                await cloudTable.ExecuteAsync(tableOperation);

            tableResult.EnsureSuccessStatusCode();
        }

        public async Task DeleteByIdAsync(
            string id)
        {
            var toDoEntity =
                await this.GetByIdAsync(id);

            await this.DeleteAsync(toDoEntity);
        }

        public async Task<ToDoEntity> GetByIdAsync(
            string id)
        {
            try
            {
                var toDoEntity =
                    await this.GetByIdAsync(id, _primaryCloudTable);

                return toDoEntity;
            }
            catch
            {
                if (this.AutoFailover)
                {
                    var toDoEntity =
                        await this.GetByIdAsync(id, _secondaryCloudTable);

                    return toDoEntity;
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task<ToDoEntity> GetByIdAsync(
            string id,
            CloudTable cloudTable)
        {
            var tableOperation =
                TableOperation.Retrieve<ToDoEntity>(id, id);

            var tableResult =
                await cloudTable.ExecuteAsync(tableOperation);

            tableResult.EnsureSuccessStatusCode();

            return tableResult.Result as ToDoEntity;
        }

        // https://stackoverflow.com/questions/26257822/azure-table-query-async-continuation-token-always-returned

        public async Task<IEnumerable<ToDoEntity>> ListAsync(
            string query = null)
        {
            try
            {
                var toDoEntityList =
                    await this.ListAsync(query, _primaryCloudTable);

                return toDoEntityList;
            }
            catch
            {
                if (this.AutoFailover)
                {
                    var toDoEntityList =
                        await this.ListAsync(query, _secondaryCloudTable);

                    return toDoEntityList;
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task<IEnumerable<ToDoEntity>> ListAsync(
            string query,
            CloudTable cloudTable)
        {
            var tableQuery =
                new TableQuery<ToDoEntity>();

            if (!string.IsNullOrWhiteSpace(query))
            {
                tableQuery =
                    new TableQuery<ToDoEntity>().Where(query);
            }

            var toDoEntityList =
                new List<ToDoEntity>();

            var continuationToken =
                default(TableContinuationToken);

            do
            {
                var tableQuerySegement =
                    await cloudTable.ExecuteQuerySegmentedAsync(tableQuery, continuationToken);

                continuationToken =
                    tableQuerySegement.ContinuationToken;

                toDoEntityList.AddRange(tableQuerySegement.Results);
            }
            while (continuationToken != null);

            return toDoEntityList;
        }

        public async Task UpdateAsync(
            ToDoEntity toDoEntity)
        {
            try
            {
                await this.UpdateAsync(toDoEntity, _primaryCloudTable);
            }
            catch
            {
                if (this.AutoFailover)
                {
                    await this.UpdateAsync(toDoEntity, _secondaryCloudTable);
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task UpdateAsync(
            ToDoEntity toDoEntity,
            CloudTable cloudTable)
        {
            var tableOperation =
                TableOperation.InsertOrReplace(toDoEntity);

            var tableResult =
                await cloudTable.ExecuteAsync(tableOperation);

            tableResult.EnsureSuccessStatusCode();
        }
    }
}

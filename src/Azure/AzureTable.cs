namespace LostTech.NKeyValue
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    public sealed class AzureTable: IConcurrentKeyValueStore<string, string, IDictionary<string, object>>
    {
        readonly CloudTable table;

        private AzureTable(CloudTable table)
        {
            this.table = table ?? throw new ArgumentNullException(nameof(table));
        }

        public async Task<IDictionary<string, object>> Get(string key)
        {
            var (found, result) = await this.TryGet(key).ConfigureAwait(false);
            return found ? result : throw new KeyNotFoundException();
        }

        public async Task<(bool, IDictionary<string, object>)> TryGet(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            var query = MakeQueryByKey(key);
            var resultSet = await this.table.ExecuteQuerySegmentedAsync(query, null).ConfigureAwait(false);
            var result = resultSet.Results.FirstOrDefault();
            if (result == null)
                return (false, null);
            return (true, result.Properties.ToDictionary(kv => kv.Key, kv => kv.Value.PropertyAsObject));
        }

        private static TableQuery MakeQueryByKey(string key)
        {
            return new TableQuery().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition(nameof(ITableEntity.RowKey), QueryComparisons.Equal, key),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition(nameof(ITableEntity.PartitionKey), QueryComparisons.Equal, key)));
        }

        public Task Put(string key, IDictionary<string, object> value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));


            var entity = new AzureTableEntity(key, value);
            return this.table.ExecuteAsync(TableOperation.InsertOrReplace(entity));
        }

        public static async Task<AzureTable> OpenOrCreate(string accountConnectionString, string tableName)
        {
            if (string.IsNullOrEmpty(accountConnectionString))
                throw new ArgumentNullException(nameof(accountConnectionString));
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException(nameof(tableName));

            var account = CloudStorageAccount.Parse(accountConnectionString);
            var tableClient = account.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync().ConfigureAwait(false);
            return new AzureTable(table);
        }

        public async Task<TaggedEntry<string, IDictionary<string, object>>> TryGetTagged(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            var query = MakeQueryByKey(key);
            var resultSet = await this.table.ExecuteQuerySegmentedAsync(query, null).ConfigureAwait(false);
            var result = resultSet.Results.FirstOrDefault();
            if (result == null)
                return null;

            return new TaggedEntry<string, IDictionary<string, object>>
            {
                Tag = result.ETag,
                Value = result.Properties.ToDictionary(kv => kv.Key, kv => kv.Value.PropertyAsObject),
            };
        }

        public async Task<bool> Put(string key, IDictionary<string, object> value, string tag)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var entity = new AzureTableEntity(key, value);
            TableOperation operation;
            if (tag == null)
            {
                operation = TableOperation.Insert(entity);
            }
            else
            {
                entity.ETag = tag;
                operation = TableOperation.InsertOrReplace(entity);
            }
            try
            {
                await this.table.ExecuteAsync(TableOperation.InsertOrReplace(entity)).ConfigureAwait(false);
                return true;
            }
            catch (StorageException exception) 
                when (exception.RequestInformation.HttpStatusCode == (int)HttpStatusCode.PreconditionFailed)
            {
                return false;
            }
        }
    }
}
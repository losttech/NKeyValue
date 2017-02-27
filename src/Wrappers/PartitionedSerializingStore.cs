namespace LostTech.Storage.Wrappers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    class PartitionedSerializingStore<TPartition, TRow, TValue, TOldValue> : IPartitionedKeyValueStore<TPartition, TRow, TValue>
    {
        readonly IPartitionedKeyValueStore<TPartition, TRow, TOldValue> store;
        readonly Func<TOldValue, TValue> deserializer;

        public PartitionedSerializingStore(IPartitionedKeyValueStore<TPartition, TRow, TOldValue> store,
                                           Func<TOldValue, TValue> deserializer)
        {
            this.store = store ?? throw new ArgumentNullException(nameof(store));
            this.deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        }

        public int? PageSizeLimit => this.store.PageSizeLimit;

        public async Task<PagedQueryResult<KeyValuePair<PartitionedKey<TPartition, TRow>, TValue>>> Query(
            Range<TPartition> partitionRange, Range<TRow> rowRange,
            int? pageSize = default(int?), object continuationToken = null)
        {
            var page = await this.store.Query(partitionRange, rowRange, pageSize, continuationToken).ConfigureAwait(false);
            return page.Select(entry => new KeyValuePair<PartitionedKey<TPartition, TRow>, TValue>(
                key: entry.Key,
                value: this.deserializer(entry.Value)));
        }
    }
}

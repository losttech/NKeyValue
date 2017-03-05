namespace LostTech.Storage.Wrappers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    class PartitionedKeySerializingStore<TPartition, TOldPartition, TRow, TOldRow, TValue, TContinuation>
        : IPartitionedKeyValueStore<TPartition, TRow, TValue, TContinuation>
        where TContinuation: class
    {
        readonly IPartitionedKeyValueStore<TOldPartition, TOldRow, TValue, TContinuation> store;
        readonly Func<TPartition, TOldPartition> partitionSerializer;
        readonly Func<TRow, TOldRow> rowSerializer;
        readonly Func<TOldPartition, TPartition> partitionDeserializer;
        readonly Func<TOldRow, TRow> rowDeserializer;

        public PartitionedKeySerializingStore(IPartitionedKeyValueStore<TOldPartition, TOldRow, TValue, TContinuation> store,
                Func<TPartition, TOldPartition> partitionSerializer,
                Func<TRow, TOldRow> rowSerializer,
                Func<TOldPartition, TPartition> partitionDeserializer,
                Func<TOldRow, TRow> rowDeserializer)
        {
            this.store = store ?? throw new ArgumentNullException(nameof(store));
            this.partitionSerializer = partitionSerializer ?? throw new ArgumentNullException(nameof(partitionSerializer));
            this.partitionDeserializer = partitionDeserializer ?? throw new ArgumentNullException(nameof(partitionDeserializer));
            this.rowSerializer = rowSerializer ?? throw new ArgumentNullException(nameof(rowSerializer));
            this.rowDeserializer = rowDeserializer ?? throw new ArgumentNullException(nameof(rowDeserializer));
        }

        public int? PageSizeLimit => this.store.PageSizeLimit;

        public async Task<PagedQueryResult<KeyValuePair<PartitionedKey<TPartition, TRow>, TValue>, TContinuation>> Query(
            Range<TPartition> partitionRange, Range<TRow> rowRange,
            int? pageSize = default(int?), TContinuation continuationToken = null)
        {
            var oldPartitionRange = partitionRange.Select(this.partitionSerializer);
            var oldRowRange = rowRange.Select(this.rowSerializer);
            var page = await this.store.Query(oldPartitionRange, oldRowRange, pageSize, continuationToken).ConfigureAwait(false);
            return page.Select(entry => new KeyValuePair<PartitionedKey<TPartition, TRow>, TValue>(
                key: new PartitionedKey<TPartition, TRow>(
                    partition: this.partitionDeserializer(entry.Key.Partition),
                    row: this.rowDeserializer(entry.Key.Row)),
                value: entry.Value
            ));
        }
    }
}

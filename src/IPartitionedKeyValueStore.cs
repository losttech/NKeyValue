namespace LostTech.Storage
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPartitionedKeyValueStore<TPartition, TRow, TValue>
    {
        int? PageSizeLimit { get; }
        Task<PagedQueryResult<KeyValuePair<PartitionedKey<TPartition, TRow>, TValue>>>
            Query(Range<TPartition> partitionRange, Range<TRow> rowRange,
                  int? pageSize = null, object continuationToken = null);
    }
}

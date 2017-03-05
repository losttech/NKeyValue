namespace LostTech.Storage
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPartitionedKeyValueStore<TPartition, TRow, TValue, TContinuation>
        where TContinuation: class
    {
        int? PageSizeLimit { get; }
        Task<PagedQueryResult<KeyValuePair<PartitionedKey<TPartition, TRow>, TValue>, TContinuation>>
            Query(Range<TPartition> partitionRange, Range<TRow> rowRange,
                  int? pageSize = null, TContinuation continuationToken = null);
    }
}

namespace LostTech.Storage
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPartitionConcurrentVersionedKeyValueStore<TPartition, TRow, TVersion, TValue>
    {
        Task<bool> Put(TPartition partition, KeyValuePair<TRow, VersionedEntry<TVersion, TValue>>[] entities);
        Task<bool> Delete(TPartition partition, KeyValuePair<TRow, TVersion>[] entities);
    }
}

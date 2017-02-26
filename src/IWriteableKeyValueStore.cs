namespace LostTech.Storage
{
    using System.Threading.Tasks;

    /// <summary>
    /// Represents asynchronous readable and writeable key-value store
    /// </summary>
    /// <typeparam name="TKey">Type of keys</typeparam>
    /// <typeparam name="TValue">Type of values</typeparam>
    public interface IWriteableKeyValueStore<in TKey, TValue> : IKeyValueStore<TKey, TValue>
    {
        /// <summary>
        /// Sets the value for the specified key.
        /// </summary>
        /// <param name="key">The key to set value for</param>
        /// <param name="value">New value for the key</param>
        Task Put(TKey key, TValue value);
        /// <summary>
        /// Attempts to delete value with the specified key. If possible, returns if something was actually deleted.
        /// </summary>
        /// <param name="key">Key to remove</param>
        Task<bool?> Delete(TKey key);
    }
}

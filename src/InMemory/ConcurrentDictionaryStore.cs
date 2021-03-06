namespace LostTech.Storage.InMemory
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LostTech.Storage.ForwardCompatibility;

    /// <summary>
    /// Implements in-memory store, backed by <see cref="ConcurrentDictionary{TKey,TValue}"/>
    /// </summary>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <remarks>Instances of this class are thread-safe</remarks>
    public sealed class ConcurrentDictionaryStore<TKey, TValue> : IConcurrentVersionedKeyValueStore<TKey, TValue>
    {
        readonly ConcurrentDictionary<TKey, VersionedEntry<object, TValue>> store = new ConcurrentDictionary<TKey, VersionedEntry<object, TValue>>();
        public Task<TValue> Get(TKey key) => 
            this.store.TryGetValue(key, out VersionedEntry<object, TValue> value) 
                ? Task.FromResult(value.Value) 
                : TaskEx.FromException<TValue>(new KeyNotFoundException());

        public Task<(bool, TValue)> TryGet(TKey key)
        {
            bool found = this.store.TryGetValue(key, out VersionedEntry<object, TValue> value);
            return Task.FromResult((found, (found ? value.Value : default(TValue))));
        }

        public Task<VersionedEntry<object, TValue>> TryGetVersioned(TKey key) =>
            Task.FromResult(this.store.TryGetValue(key, out VersionedEntry<object, TValue> value) ? value : null);

        public Task Put(TKey key, TValue value)
        {
            var entry = new VersionedEntry<object, TValue>
            {
                Version = new object(),
                Value = value,
            };
            this.store.AddOrUpdate(key, entry, (_,__) => entry);
            return Task.FromResult(true);
        }

        public Task<(bool, object)> Put(TKey key, TValue value, object version)
        {
            var oldEntry = new VersionedEntry<object, TValue> {Version = version};
            var entry = new VersionedEntry<object, TValue>
            {
                Version = new object(),
                Value = value,
            };
            var updated = version == null
                ? this.store.TryAdd(key, entry)
                : this.store.TryUpdate(key, entry, oldEntry);
            return Task.FromResult((updated, entry.Version));
        }

        public Task<bool> Delete(TKey key, object versionToDelete)
        {
            bool hadKey = this.store.TryRemove(key, out var entry);
            if (!hadKey)
                return Task.FromResult(false);

            if (entry.Version != versionToDelete) {
                this.store.AddOrUpdate(key, entry, (_, __) => entry);
                return Task.FromResult(false);
            }
            else {
                return Task.FromResult(true);
            }
        }
        public Task<bool?> Delete(TKey key) => Task.FromResult((bool?)this.store.TryRemove(key, out var _));
    }
}

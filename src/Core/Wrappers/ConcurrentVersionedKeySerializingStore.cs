﻿namespace LostTech.NKeyValue.Wrappers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    class ConcurrentVersionedKeySerializingStore<TKey, TVersion, TValue> :
        WriteableKeySerializingStore<TKey, TValue>,
        IConcurrentVersionedKeyValueStore<TKey, TVersion, TValue>
    {
        readonly IConcurrentVersionedKeyValueStore<string, TVersion, TValue> store;

        public ConcurrentVersionedKeySerializingStore(IConcurrentVersionedKeyValueStore<string, TVersion, TValue> store,
            Func<TKey, string> keySerializer) : base(store, keySerializer)
        {
            this.store = store;
        }

        public Task<bool> Put(TKey key, TValue value, TVersion versionToUpdate)
        {
            var serializedKey = this.keySerializer(key);
            return this.store.Put(serializedKey, value, versionToUpdate);
        }

        public Task<VersionedEntry<TVersion, TValue>> TryGetVersioned(TKey key)
        {
            var serializedKey = this.keySerializer(key);
            return this.store.TryGetVersioned(serializedKey);
        }
    }
}

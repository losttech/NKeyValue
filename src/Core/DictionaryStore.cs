﻿namespace LostTech.NKeyValue
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public sealed class DictionaryStore<TKey, TValue> : IWriteableKeyValueStore<TKey, TValue>
    {
        readonly Dictionary<TKey, TValue> store = new Dictionary<TKey,TValue>();

        public Task<TValue> Get(TKey key)
        {
            if (this.store.TryGetValue(key, out var value))
                return Task.FromResult(value);

            return Task.FromException<TValue>(new KeyNotFoundException());
        }

        public Task Put(TKey key, TValue value)
        {
            this.store[key] = value;
            return Task.CompletedTask;
        }
    }
}

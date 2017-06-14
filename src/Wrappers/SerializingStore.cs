namespace LostTech.Storage.Wrappers
{
    using System;
    using System.Threading.Tasks;

    class SerializingStore<TKey, TValue, TOldValue> : IKeyValueStore<TKey, TValue>
    {
        readonly IKeyValueStore<TKey, TOldValue> store;
        protected readonly Func<TOldValue, TValue> deserializer;

        public SerializingStore(IKeyValueStore<TKey, TOldValue> store, Func<TOldValue, TValue> deserializer)
        {
            this.store = store ?? throw new ArgumentNullException(nameof(store));
            this.deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        }

        public async Task<TValue> Get(TKey key)
        {
            var serialized = await this.store.Get(key).ConfigureAwait(false);
            return this.deserializer(serialized);
        }

        public async Task<(bool, TValue)> TryGet(TKey key)
        {
            var (found, serialized) = await this.store.TryGet(key).ConfigureAwait(false);
            return found
                ? (true, this.deserializer(serialized))
                : (false, default(TValue));
        }
    }
}

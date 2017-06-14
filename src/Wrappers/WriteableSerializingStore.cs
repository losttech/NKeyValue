namespace LostTech.Storage.Wrappers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    class WriteableSerializingStore<TKey, TValue> : SerializingStore<TKey, TValue, IDictionary<string, object>>,
        IWriteableKeyValueStore<TKey, TValue>
    {
        readonly IWriteableKeyValueStore<TKey, IDictionary<string, object>> backingStore;
        readonly Action<TValue, IDictionary<string, object>> serializer;
        protected readonly IDictionary<string, object> serializedValue = new Dictionary<string, object>();

        public WriteableSerializingStore(IWriteableKeyValueStore<TKey, IDictionary<string, object>> backingStore, 
            Func<IDictionary<string, object>, TValue> deserializer,
            Action<TValue, IDictionary<string, object>> serializer)
            : base(backingStore, deserializer)
        {
            this.backingStore = backingStore;
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public Task<bool?> Delete(TKey key) => this.backingStore.Delete(key);

        public Task Put(TKey key, TValue value)
        {
            this.serializedValue.Clear();
            this.serializer(value, this.serializedValue);
            return this.backingStore.Put(key, this.serializedValue);
        }

        protected void Serialize(TValue value)
        {
            this.serializedValue.Clear();
            this.serializer(value, this.serializedValue);
        }
    }

    class WriteableSerializingStore<TKey, TValue, TOldValue> : SerializingStore<TKey, TValue, TOldValue>,
        IWriteableKeyValueStore<TKey, TValue>
    {
        readonly IWriteableKeyValueStore<TKey, TOldValue> backingStore;
        readonly Func<TValue, TOldValue> serializer;

        public WriteableSerializingStore(IWriteableKeyValueStore<TKey, TOldValue> backingStore,
            Func<TOldValue, TValue> deserializer,
            Func<TValue, TOldValue> serializer)
            : base(backingStore, deserializer)
        {
            this.backingStore = backingStore;
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public Task<bool?> Delete(TKey key) => this.backingStore.Delete(key);

        public Task Put(TKey key, TValue value)
        {
            var serializedValue = this.serializer(value);
            return this.backingStore.Put(key, serializedValue);
        }
    }
}

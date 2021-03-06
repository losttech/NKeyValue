﻿namespace LostTech.Storage
{
    using System;
    using System.Collections.Generic;
    using LostTech.Storage.Wrappers;

    public static class KeyValueStore
    {
        /// <summary>
        /// Wraps <see cref="String"/>-keyed store to use <typeparamref name="TKey"/> for keys, using the provided key serializer.
        /// </summary>
        /// <typeparam name="TKey">Type of keys to use</typeparam>
        /// <typeparam name="TValue">Type of store values</typeparam>
        /// <param name="store"><see cref="String"/>-keyed store to wrap</param>
        /// <param name="keySerializer">Serializer, that converts <typeparamref name="TKey"/> keys into string keys</param>
        public static IWriteableKeyValueStore<TKey, TValue> WithKey<TKey, TOldKey, TValue>(
            this IWriteableKeyValueStore<TOldKey, TValue> store, Func<TKey, TOldKey> keySerializer) =>
            new WriteableKeySerializingStore<TKey, TOldKey, TValue>(store, keySerializer);

        /// <summary>
        /// Wraps dictionary-valued store to use <typeparamref name="TValue"/> for values, using the provided codec.
        /// </summary>
        /// <typeparam name="TKey">Type of store keys</typeparam>
        /// <typeparam name="TValue">Type of values to use</typeparam>
        /// <param name="store">Dictionary-valued store to wrap</param>
        /// <param name="deserializer">Deserializer (decoder) for values</param>
        /// <param name="serializer">Serializer (encoder) for values</param>
        /// <returns></returns>
        public static IWriteableKeyValueStore<TKey, TValue> WithValue<TKey, TValue>(
            this IWriteableKeyValueStore<TKey, IDictionary<string, object>> store,
            Func<IDictionary<string, object>, TValue> deserializer,
            Action<TValue, IDictionary<string, object>> serializer)
            => new WriteableSerializingStore<TKey, TValue>(store, deserializer, serializer);

        public static IWriteableKeyValueStore<TKey, TValue> WithValue<TKey, TValue, TOldValue>(
            this IWriteableKeyValueStore<TKey, TOldValue> store,
            Func<TOldValue, TValue> deserializer,
            Func<TValue, TOldValue> serializer)
            => new WriteableSerializingStore<TKey,TValue, TOldValue>(store, deserializer, serializer);

        /// <summary>
        /// Wraps <see cref="String"/>-keyed store to use <typeparamref name="TKey"/> for keys, using the provided key serializer.
        /// </summary>
        /// <typeparam name="TKey">Type of keys to use</typeparam>
        /// <typeparam name="TValue">Type of store values</typeparam>
        /// <param name="store"><see cref="String"/>-keyed store to wrap</param>
        /// <param name="keySerializer">Serializer, that converts <typeparamref name="TKey"/> keys into string keys</param>
        public static IConcurrentVersionedKeyValueStore<TKey, TVersion, TValue> WithKey<TKey, TOldKey, TVersion, TValue>(
            this IConcurrentVersionedKeyValueStore<TOldKey, TVersion, TValue> store, Func<TKey, TOldKey> keySerializer) =>
            new ConcurrentVersionedKeySerializingStore<TKey, TOldKey, TVersion, TValue>(store, keySerializer);

        /// <summary>
        /// Wraps dictionary-valued store to use <typeparamref name="TValue"/> for values, using the provided codec.
        /// </summary>
        /// <typeparam name="TKey">Type of store keys</typeparam>
        /// <typeparam name="TValue">Type of values to use</typeparam>
        /// <param name="store">Dictionary-valued store to wrap</param>
        /// <param name="deserializer">Deserializer (decoder) for values</param>
        /// <param name="serializer">Serializer (encoder) for values</param>
        /// <returns></returns>
        public static IConcurrentVersionedKeyValueStore<TKey, TVersion, TValue> WithValue<TKey, TVersion, TValue>(
            this IConcurrentVersionedKeyValueStore<TKey, TVersion, IDictionary<string, object>> store,
            Func<IDictionary<string, object>, TValue> deserializer,
            Action<TValue, IDictionary<string, object>> serializer)
            => new ConcurrentVersionedSerializingStore<TKey, TVersion, TValue>(store, deserializer, serializer);

        public static IConcurrentVersionedKeyValueStore<TKey, TVersion, TValue> 
            WithRowKeyAsPartitionKey<TStore, TKey, TVersion, TValue>(
                this IConcurrentVersionedKeyValueStore<PartitionedKey<TKey, TKey>, TVersion, TValue> partitionedStore)
            => partitionedStore.WithKey((TKey key) => new PartitionedKey<TKey, TKey>(key, key));

        public static IPartitionedKeyValueStore<TPartition, TRow, TValue, TContinuation>
            WithKey<TPartition, TOldPartition, TRow, TOldRow, TValue, TContinuation>(
                this IPartitionedKeyValueStore<TOldPartition, TOldRow, TValue, TContinuation> store,
                Func<TPartition, TOldPartition> partitionSerializer,
                Func<TRow, TOldRow> rowSerializer,
                Func<TOldPartition, TPartition> partitionDeserializer,
                Func<TOldRow, TRow> rowDeserializer)
            where TContinuation: class =>
            new PartitionedKeySerializingStore<TPartition, TOldPartition, TRow, TOldRow, TValue, TContinuation>(
                store, partitionSerializer, rowSerializer, partitionDeserializer, rowDeserializer);
        public static IPartitionedKeyValueStore<TPartition, TRow, TValue, TContinuation>
            WithValue<TPartition, TRow, TValue, TOldValue, TContinuation>(
                this IPartitionedKeyValueStore<TPartition, TRow, TOldValue, TContinuation> store,
                Func<TOldValue, TValue> deserializer
            ) where TContinuation : class => 
            new PartitionedSerializingStore<TPartition,TRow,TValue,TOldValue,TContinuation>(store, deserializer);
    }
}

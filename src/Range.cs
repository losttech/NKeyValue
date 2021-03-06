﻿namespace LostTech.Storage
{
    using System;

    public struct Range<T>
    {
        public Range(T start, T end)
        {
            this.Start = start;
            this.End = end;
        }
        public T Start { get; }
        public T End { get; }

        public Range<TNew> Select<TNew>(Func<T, TNew> selector) =>
            selector == null
            ? throw new ArgumentNullException(nameof(selector))
            : new Range<TNew>(start: selector(this.Start), end: selector(this.End));
    }

    public static class Range
    {
        public static Range<T> SingleElement<T>(T value) => new Range<T>(value, value);
        public static Range<T> Create<T>(T start, T end) => new Range<T>(start, end);
    }
}

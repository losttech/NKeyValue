namespace LostTech.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class PagedQueryResult<T, TContinuation>
        where TContinuation: class
    {
        public PagedQueryResult(IReadOnlyList<T> results, TContinuation nextPageToken = null)
        {
            this.Results = results ?? throw new ArgumentNullException(nameof(results));
            this.NextPageToken = nextPageToken;
        }

        public TContinuation NextPageToken { get; }
        public IReadOnlyList<T> Results { get; }


        public PagedQueryResult<TNew, TContinuation> Select<TNew>(Func<T, TNew> deserializer) =>
            deserializer == null
            ? throw new ArgumentNullException(nameof(deserializer))
            : new PagedQueryResult<TNew, TContinuation>(
                results: this.Results.Select(deserializer).ToArray(),
                nextPageToken: this.NextPageToken);
    }
}

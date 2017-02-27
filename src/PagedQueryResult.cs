namespace LostTech.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class PagedQueryResult<T>
    {
        public PagedQueryResult(IReadOnlyList<T> results, object nextPageToken = null)
        {
            this.Results = results ?? throw new ArgumentNullException(nameof(results));
            this.NextPageToken = nextPageToken;
        }

        public object NextPageToken { get; }
        public IReadOnlyList<T> Results { get; }


        public PagedQueryResult<TNew> Select<TNew>(Func<T, TNew> deserializer) =>
            deserializer == null
            ? throw new ArgumentNullException(nameof(deserializer))
            : new PagedQueryResult<TNew>(
                results: this.Results.Select(deserializer).ToArray(),
                nextPageToken: this.NextPageToken);
    }
}

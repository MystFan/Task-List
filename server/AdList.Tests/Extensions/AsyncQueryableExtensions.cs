using AdList.Tests.Abstract;

namespace AdList.Tests.Extensions
{
    internal static class AsyncQueryableExtensions
    {
        internal static IQueryable<TElement> AsAsyncQueryable<TElement>(this IEnumerable<TElement> source)
        {
            return new DbAsyncEnumerable<TElement>(source);
        }
    }
}

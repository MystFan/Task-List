using System.Linq.Expressions;

namespace AdList.Tests.Abstract
{
    internal class DbAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public DbAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        {
        }

        public DbAsyncEnumerable(Expression expression)
            : base(expression)
        {
        }

        IQueryProvider IQueryable.Provider
        {
            get => new AsyncQueryProvider<T>(this);
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new())
        {
            return new DbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }
    }
}

namespace AdList.Tests.Abstract
{
    internal class DbAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public DbAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }


        public ValueTask<bool> MoveNextAsync()
        {
            return ValueTask.FromResult(_inner.MoveNext());
        }

        public T Current
        {
            get => _inner.Current;
        }

        public async ValueTask DisposeAsync()
        {
            if (_inner is IAsyncDisposable innerAsyncDisposable)
            {
                await innerAsyncDisposable.DisposeAsync();
            }
            else
            {
                _inner.Dispose();
            }
        }
    }
}

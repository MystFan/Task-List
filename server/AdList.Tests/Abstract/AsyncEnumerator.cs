namespace AdList.Tests.Abstract
{
    internal class AsyncEnumerator<T> : IAsyncEnumerator<T>
    {
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

        private readonly IEnumerator<T> _inner;

        public AsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public void Dispose()
        {
            _inner.Dispose();
        }


        public ValueTask<bool> MoveNextAsync()
        {
            return ValueTask.FromResult(_inner.MoveNext());
        }

        public T Current
        {
            get => _inner.Current;
        }

        T IAsyncEnumerator<T>.Current
        {
            get => Current;
        }
    }
}

using Moq.Language.Flow;

namespace AdList.Tests.Extensions
{
    public static class MockExtensions
    {
        public static IReturnsResult<TMock> ReturnsCollectionOf<TMock, TResult>(this ISetup<TMock, IQueryable<TResult>> setup, params TResult[] results)
            where TMock : class
        {
            return setup.Returns(results.AsAsyncQueryable());
        }

        public static IReturnsResult<TMock> ReturnsCollectionOf<TMock, TResult>(this ISetup<TMock, IQueryable<TResult>> setup,
            IEnumerable<TResult> results)
            where TMock : class
        {
            return setup.Returns(results.AsAsyncQueryable());
        }

        public static IReturnsResult<TMock> ReturnsCollectionOf<TMock, TResult>(this ISetup<TMock, IQueryable<TResult>> setup,
            Func<IEnumerable<TResult>> results)
            where TMock : class
        {
            return setup.Returns(() => results().AsAsyncQueryable());
        }

        public static IReturnsResult<TMock> ReturnsCollectionOf<TMock, TResult, TArg>(this ISetup<TMock, IQueryable<TResult>> setup,
            Func<TArg, IEnumerable<TResult>> results)
            where TMock : class
        {
            return setup.Returns<TArg>(arg => results(arg).AsAsyncQueryable());
        }

        public static IReturnsResult<TMock> ReturnsCollectionOfAsync<TMock, TResult>(this ISetup<TMock, Task<IQueryable<TResult>>> setup,
            params TResult[] results)
            where TMock : class
        {
            return setup.ReturnsAsync(results.AsAsyncQueryable());
        }
    }
}

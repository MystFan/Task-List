using AdList.Application.Commands.GetTasksCommand;
using System.Linq.Expressions;

namespace AdList.Application.Infrastructure
{
    public static class QueryOrderer
    {
        public static IQueryable<T> ApplyOrdering<T>(IQueryable<T> query, IEnumerable<SortOption> sortOptions)
        {
            if (sortOptions == null || !sortOptions.Any())
            {
                return query;
            }

            IOrderedQueryable<T>? orderedQuery = null;

            foreach (var sort in sortOptions)
            {
                var parameter = Expression.Parameter(typeof(T), "x");

                Expression propertyAccess = parameter;

                foreach (var member in sort.Name.Split('.'))
                {
                    propertyAccess = Expression.PropertyOrField(propertyAccess, member);
                }

                var lambda = Expression.Lambda(propertyAccess, parameter);

                string methodName;

                if (orderedQuery == null)
                {
                    methodName = sort.Direction == SortDirection.Desc
                        ? "OrderByDescending"
                        : "OrderBy";
                }
                else
                {
                    methodName = sort.Direction == SortDirection.Desc
                        ? "ThenByDescending"
                        : "ThenBy";
                }

                var method = typeof(Queryable)
                    .GetMethods()
                    .First(m => m.Name == methodName && m.GetParameters().Length == 2);

                var genericMethod = method.MakeGenericMethod(typeof(T), propertyAccess.Type);

                orderedQuery = (IOrderedQueryable<T>)genericMethod.Invoke(
                    null,
                    new object[] { orderedQuery ?? query, lambda })!;
            }

            return orderedQuery ?? query;
        }
    }
}

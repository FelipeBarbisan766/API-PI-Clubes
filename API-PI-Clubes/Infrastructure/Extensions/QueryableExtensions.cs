using System.Linq.Expressions;

namespace API_PI_Clubes.Infrastructure.Extensions;

internal static class QueryableExtensions
{
    public static IOrderedQueryable<T> OrderByDirection<T, TKey>(
        this IQueryable<T> source,
        Expression<Func<T, TKey>> keySelector,
        bool descending)
        => descending
            ? source.OrderByDescending(keySelector)
            : source.OrderBy(keySelector);
}
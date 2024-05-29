using System.Linq.Expressions;
using WarehouseManagement.Core.DTOs;

namespace WarehouseManagement.Infrastructure.Data.Common
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Paginate<T>(
            this IQueryable<T> query,
            PaginationParameters parameters,
            Expression<Func<T, bool>> filterExpression
        )
        {
            if (!string.IsNullOrEmpty(parameters.SearchQuery) && filterExpression != null)
            {
                query = query.Where(filterExpression);
            }

            return query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize);
        }
    }
}

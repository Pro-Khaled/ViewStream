using System.Linq.Expressions;

namespace ViewStream.Application.Common
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> OrderByPropertyName<T>(this IQueryable<T> source, string propertyName, bool isDescending)
        {
            if (string.IsNullOrWhiteSpace(propertyName)) return source;

            var type = typeof(T);
            var property = type.GetProperty(propertyName, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            
            if (property == null) return source;

            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);

            string methodName = isDescending ? "OrderByDescending" : "OrderBy";
            
            var resultExpression = Expression.Call(typeof(Queryable), methodName, 
                new Type[] { type, property.PropertyType },
                source.Expression, Expression.Quote(orderByExpression));

            return source.Provider.CreateQuery<T>(resultExpression);
        }
    }
}

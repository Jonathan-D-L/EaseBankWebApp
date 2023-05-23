using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace UtilityLibrary
{
    public static class ColumnSorter
    {
        public static IQueryable<T> SortColumn<T>(this IQueryable<T> query, string? sortColumn, string sortOrder) where T : class
        {
            if (sortColumn == null) return query;
            var colNames = typeof(T).GetProperties().Select(property => property.Name.ToLower()).ToList();
            return (from colName in colNames
                    where colName == sortColumn.ToLower()
                    let parameter = Expression.Parameter(typeof(T), "s")
                    let property = Expression.Property(parameter, colName)
                    let convert = Expression.Convert(property, typeof(object))
                    select Expression.Lambda<Func<T, object>>(convert, parameter))
                    .Aggregate(query, (current, lambda) => sortOrder == "asc"
                    ? current.OrderBy(lambda)
                    : current.OrderByDescending(lambda));
        }
    }
}

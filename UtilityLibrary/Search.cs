using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace UtilityLibrary
{
    public class Search
    {
        public static List<T> ScopedDataSearch<T>(IQueryable<T> context, string q) where T : class
        {
            if (string.IsNullOrWhiteSpace(q)) return new List<T>();
            var qArray = q.Split(" ");
            var query = context.ToList()
                .Where(x => qArray.All(q => typeof(T).GetProperties()
                    .Where(p => !p.PropertyType.IsGenericType ||
                                !p.PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(ICollection<>)))
                    .Select(p => p.GetValue(x)?.ToString() ?? "")
                    .Any(n => n.ToLower().Equals(q.ToLower()))));
            if (!query.Any())
            {
                query = context.ToList()
                    .Where(x => qArray.All(q => typeof(T).GetProperties()
                        .Where(p => !p.PropertyType.IsGenericType ||
                                    !p.PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(ICollection<>)))
                        .Select(p => p.GetValue(x)?.ToString() ?? "")
                        .Any(n => n.ToLower().StartsWith(q.ToLower()))));
            }
            if (!query.Any())
            {
                query = context.ToList()
                    .Where(x => qArray.All(q => typeof(T).GetProperties()
                        .Where(p => !p.PropertyType.IsGenericType ||
                                    !p.PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(ICollection<>)))
                        .Select(p => p.GetValue(x)?.ToString() ?? "")
                        .Any(n => n.ToLower().EndsWith(q.ToLower()))));
            }
            if (!query.Any())
            {
                query = context.ToList()
                    .Where(x => qArray.All(q => typeof(T).GetProperties()
                        .Where(p => !p.PropertyType.IsGenericType ||
                                    !p.PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(ICollection<>)))
                        .Select(p => p.GetValue(x)?.ToString() ?? "")
                        .Any(n => n.ToLower().Contains(q.ToLower()))));
            }
            return query.ToList();
        }
    }
}

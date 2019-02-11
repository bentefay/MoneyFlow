using System;
using System.Collections.Generic;
using System.Linq;

namespace Web.Utils.Extensions
{
    public static class EnumerableExtensions
    {
        public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> @this) =>
            @this is IReadOnlyList<T> readOnlyList ?
                readOnlyList :
                @this.ToList();

        public static IReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> @this) =>
            @this is IReadOnlyCollection<T> readOnlyCollection ?
                readOnlyCollection :
                @this.ToList();

        public static string Join<T>(this IEnumerable<T> @this, string separator) =>
            string.Join(separator, @this);
        
        public static (IEnumerable<T> True, IEnumerable<T> False) Partition<T>(this IEnumerable<T> @this, Func<T, bool> predicate)
        {
            var list = @this.ToList();
            return (list.Where(predicate), list.Where(item => !predicate(item)));
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> @this, T item) =>
            @this.Concat(new[] { item });
    }
}

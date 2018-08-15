using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt;

namespace Make.Utility.Extensions
{
    public static class EnumerableExtensions
    {
        public static EitherAsync<Error, Unit> Do<T>(this IEnumerable<T> source, Func<T, EitherAsync<Error, Unit>> f)
        {
            return source.Aggregate(
                Prelude.RightAsync<Error, Unit>(Task.FromResult(Prelude.unit)),
                (acc, item) => acc.Bind(_ => f(item)));
        }
        
        public static Option<T> FirstOrNone<T>(this IEnumerable<T> source)
        {
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));
                case IList<T> list:
                    return list.Count > 0 ? (Option<T>) list[0] : Prelude.None;
                default:
                    using (var enumerator = source.GetEnumerator())
                        return enumerator.MoveNext() ? (Option<T>) enumerator.Current : Prelude.None;
            }
        }
    }
}
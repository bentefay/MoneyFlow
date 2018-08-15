using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt;

namespace Make.Utility.Extensions
{
    public static class EnumerableExtensions
    {
        public static EitherAsync<Error, Unit> Do<T>(this IEnumerable<T> seq, Func<T, EitherAsync<Error, Unit>> f)
        {
            return seq.Aggregate(
                Prelude.RightAsync<Error, Unit>(Task.FromResult(Prelude.unit)),
                (acc, item) => acc.Bind(_ => f(item)));
        }
    }
}
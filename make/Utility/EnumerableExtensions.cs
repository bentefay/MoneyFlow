using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Make.Utility
{
    public static class EnumerableExtensions
    {
        public static EitherAsync<Error, Unit> ForEach<T>(this IEnumerable<T> seq, Func<T, EitherAsync<Error, Unit>> f)
        {
            return seq.Aggregate(
                RightAsync<Error, Unit>(Task.FromResult(unit)),
                (acc, item) => acc.Bind(_ => f(item)));
        }
    }
}
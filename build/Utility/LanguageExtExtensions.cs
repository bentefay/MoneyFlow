using System;
using System.Threading.Tasks;
using LanguageExt;

namespace Build.Utility
{
    public static class LanguageExtExtensions
    {
        public static OptionAsync<B> Bind<A, B>(this OptionAsync<A> option, Func<A, Option<B>> f)
        {
            return option.Bind(value => f(value).ToAsync());
        }
        
        public static OptionAsync<B> Bind<A, B>(this OptionAsync<A> option, Func<A, Task<Option<B>>> f)
        {
            return option.Bind(value => f(value).ToAsync());
        }
        
        public static EitherAsync<L, Ret> Bind<L, R, Ret>(this EitherAsync<L, R> either, Func<R, Either<L, Ret>> f)
        {
            return either.Bind(value => f(value).ToAsync());
        }
    }
}
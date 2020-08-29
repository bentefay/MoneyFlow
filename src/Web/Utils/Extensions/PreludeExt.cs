using System;
using System.Threading.Tasks;
using LanguageExt;
using Web.Types.Errors;

namespace Web.Utils.Extensions
{
    public static class PreludeExt
    {
        public static EitherAsync<TL, TR> DoLeft<TL, TR>(this EitherAsync<TL, TR> @this, Action<TL> f)
        {
            return @this.MapLeft(x =>
            {
                f(x);
                return x;
            });
        }
        
        public static EitherAsync<TL, TR2> BindEitherAsync<TL, TR, TR2>(this EitherAsync<TL, TR> @this, Func<TR, Task<Either<TL, TR2>>> func)
        {
            return @this.Bind(x => func(x).ToAsync());
        }

        public static Either<TL2, TR> Left<TL1, TL2, TR>(this Either<TL1, TR> @this, Cast<TL2> to) where TL1 : TL2
        {
            return @this.MapLeft(left => (TL2)left);
        }
        
        public static EitherAsync<TL2, TR> Left<TL1, TL2, TR>(this EitherAsync<TL1, TR> @this, Cast<TL2> to) where TL1 : TL2
        {
            return @this.MapLeft(left => (TL2)left);
        }


        public static EitherAsync<TL, TR> CreateEitherAsync<TL, TR>(this Func<Task<Either<TL, TR>>> @this)
        {
            return @this().ToAsync();
        }

        public static Either<TL, TR> Try<TL, TR>(Func<TR> func, Func<Exception, TL> onError)
        {
            try
            {
                return func();
            }
            catch (Exception e)
            {
                return onError(e);
            }
        }

        public static EitherAsync<TL, TR> TryAsync<TL, TR>(Func<Task<TR>> func, Func<Exception, TL> onError)
        {
            return CreateEitherAsync<TL, TR>(async () =>
            {
                try
                {
                    return await func();
                }
                catch (Exception e)
                {
                    return onError(e);
                }
            });
        }
    }

    public static class Cast
    {
        public static Cast<T> To<T>() => new Cast<T>();
    }

    public struct Cast<T>
    {
    }
}
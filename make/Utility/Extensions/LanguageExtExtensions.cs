using System;
using System.Threading.Tasks;
using LanguageExt;

namespace Make.Utility.Extensions
{
    public static class LanguageExtExtensions
    {
        public static EitherAsync<L, Ret> Bind<L, R, Ret>(this EitherAsync<L, R> either, Func<R, Either<L, Ret>> f)
        {
            return either.Bind(value => f(value).ToAsync());
        }
        
        public static EitherAsync<L, Ret> Bind<L, R, Ret>(this EitherAsync<L, R> either, Func<R, Task<Either<L, Ret>>> f)
        {
            return either.Bind(value => f(value).ToAsync());
        }
        
        public static Task<int> ToExitCode(this EitherAsync<Error, Unit> either)
        {
            return either.Match(
                _ =>
                {
                    Utilities.Log();
                    Utilities.LogSuccess("Build completed successfully");
                    return 0;
                },
                error =>
                {
                    Utilities.Log();
                    Utilities.LogError($"Build failed: {error.Message}");
                    error.Exception.Iter(exception =>
                    {
                        Utilities.Log();
                        Utilities.LogError($"Exception: {exception.Message}");
                        Utilities.Log();
                        Utilities.LogError(exception.StackTrace);
                    });
                    return 1;
                });
        }

        public static EitherAsync<Error, T> ToRightAsync<T>(this Task<T> task)
        {
            return Prelude.RightAsync<Error, T>(task);
        }
    }
}
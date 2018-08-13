using System;
using System.Threading.Tasks;
using LanguageExt;
using static Make.Utility.Utilities;

namespace Make.Utility
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
                    Log();
                    LogSuccess("Build completed successfully");
                    return 0;
                },
                error =>
                {
                    Log();
                    LogError($"Build failed: {error.Message}");
                    error.Exception.Iter(exception =>
                    {
                        Log();
                        LogError($"Exception: {exception.Message}");
                        Log();
                        LogError(exception.StackTrace);
                    });
                    return 1;
                });
        }
    }
}
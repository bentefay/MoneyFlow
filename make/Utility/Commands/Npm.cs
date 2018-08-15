using LanguageExt;
using static Make.Utility.Utilities;

namespace Make.Utility
{
    public static class Npm
    {
        public static EitherAsync<Error, Unit> Install(string directory)
        {
            return Execute.RunToEither(new ExecutionOptions(directory), "npm install");
        }
        
        public static EitherAsync<Error, Unit> Ci(string directory)
        {
            return Execute.RunToEither(new ExecutionOptions(directory), "npm ci");
        }
    }
}
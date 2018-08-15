using LanguageExt;
using Make.Utility.Commands.Executables;
using Make.Utility.Extensions;

namespace Make.Utility.Commands
{
    public static class Npm
    {
        public static EitherAsync<Error, Unit> Install(string directory)
        {
            return Executable.RunToEither(new ExecutionOptions(directory), "npm install");
        }

        public static EitherAsync<Error, Unit> Ci(string directory)
        {
            return
                from supportsCi in NpmMajorVersion().Map(v => v > 5).IfNone(false).ToRightAsync()
                from result in Executable.RunToEither(new ExecutionOptions(directory), supportsCi ? "npm ci" : "npm install")
                select result;
        }

        private static OptionAsync<int> NpmMajorVersion() =>
            Prelude.OptionalAsync(Executable.RunToString("npm --version"))
                .Bind(version => version.Split('.').FirstOrNone().ToAsync())
                .Bind(token => token.ParseInt().ToAsync());
    }
}
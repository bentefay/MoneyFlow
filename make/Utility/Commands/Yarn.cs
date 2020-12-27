using LanguageExt;
using Make.Utility.Commands.Executables;

namespace Make.Utility.Commands
{
    public static class Yarn
    {
        public static EitherAsync<Error, Unit> Install(string directory, ExecutionOptions options)
        {
            return Executable.RunAsEither(options.With(workingDirectory: directory), "yarn install");
        }

        public static EitherAsync<Error, Unit> Ci(string directory)
        {
            return Executable.RunAsEither(new ExecutionOptions(directory), "yarn install");
        }
    }
}
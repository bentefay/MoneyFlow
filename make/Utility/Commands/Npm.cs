using LanguageExt;
using Make.Utility.Commands.Executables;
using Make.Utility.Extensions;

namespace Make.Utility.Commands
{
    public static class Npm
    {
        public static EitherAsync<Error, Unit> Install(string directory, ExecutionOptions options)
        {
            return Executable.RunToEither(options.With(workingDirectory: directory), "npm install");
        }

        public static EitherAsync<Error, Unit> Ci(string directory)
        {
            // Change this command to "npm ci" when the TFS build agent npm version is updated to > 6
            return Executable.RunToEither(new ExecutionOptions(directory), "npm install");
        }
    }
}
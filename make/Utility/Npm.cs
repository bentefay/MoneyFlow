using LanguageExt;
using static Make.Utility.Utilities;

namespace Make.Utility
{
    public static class Npm
    {
        public static EitherAsync<Error, Unit> Install(string directory)
        {
            return CommandLine.ToEither(new CommandLineOptions(directory), "npm install");
        }

        public static EitherAsync<Error, Unit> Update(string directory)
        {
            return CommandLine.ToEither(new CommandLineOptions(directory), "npm update");
        }
    }
}
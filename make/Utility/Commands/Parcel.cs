using LanguageExt;
using Make.Utility.Commands.Executables;

namespace Make.Utility.Commands
{
    public static class Parcel
    {
        public static EitherAsync<Error, Unit> RunDev(string projectDirectory, string outputDirectory, string cacheDirectory, int logLevel, ExecutionOptions options)
        {
            return Executable.RunAsEither(options, $"{Exe(projectDirectory)} {projectDirectory}/index.html --out-dir {outputDirectory} --cache-dir {cacheDirectory} --log-level {logLevel} --open --no-autoinstall");
        }

        public static EitherAsync<Error, Unit> BuildProd(string projectDirectory, string outputDirectory, string cacheDirectory, int logLevel)
        {
            return Executable.RunAsEither($"{Exe(projectDirectory)} build {projectDirectory}/index.html --out-dir {outputDirectory} --cache-dir {cacheDirectory} --log-level {logLevel}"); // --no-autoinstall
        }

        private static string Exe(string projectDirectory)
        {
            return $"{projectDirectory}/node_modules/.bin/parcel";
        }
    }
}
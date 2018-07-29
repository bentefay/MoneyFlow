using LanguageExt;

namespace Make.Utility
{
    public static class Parcel
    {
        public static EitherAsync<Error, Unit> Build(string projectDirectory, params string[] args)
        {
            return CommandLine.RunToOption($"{Exe(projectDirectory)} build {projectDirectory}/index.html {args.Join(" ")}");
        }

        public static EitherAsync<Error, Unit> Build(string projectDirectory, string outputDirectory, string cacheDirectory, params string[] args)
        {
            return CommandLine.RunToOption($"{Exe(projectDirectory)} build {projectDirectory}/index.html --out-dir {outputDirectory} --cache-dir {cacheDirectory} {args.Join(" ")}");
        }
        
        private static string Exe(string projectDirectory)
        {
            return $"{projectDirectory}/node_modules/.bin/parcel";
        }
    }
}
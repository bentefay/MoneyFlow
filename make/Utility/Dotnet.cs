using LanguageExt;

namespace Make.Utility
{
    public static class Dotnet
    {
        public static EitherAsync<Error, Unit> Publish(string projectDirectory, string configuration, string verbosity, string outputDirectory)
        {
            return CommandLine.ToEither($"dotnet publish {projectDirectory} --configuration {configuration} --output {outputDirectory} --verbosity {verbosity}");
        }

        public static EitherAsync<Error, Unit> Test(string projectDirectory, string configuration, string verbosity, string outputDirectory, string resultDirectory, string resultsFileName)
        {
            return CommandLine.ToEither($"dotnet test {projectDirectory} --configuration {configuration} --output {outputDirectory} --results-directory {resultDirectory} --verbosity {verbosity} --logger trx;logfilename={resultsFileName}.xml");
        }
        
        public static EitherAsync<Error, Unit> Clean(string projectDirectory, string configuration, string verbosity)
        {
            return CommandLine.ToEither($"dotnet clean {projectDirectory} --configuration {configuration} --verbosity {verbosity}");
        }
       
        public static EitherAsync<Error, Unit> TestWatch(string projectDirectory)
        {
            return CommandLine.ToEither($"dotnet watch -p {projectDirectory} test");
        }
        
        public static EitherAsync<Error, Unit> RunWatch(string projectDirectory)
        {
            return CommandLine.ToEither($"dotnet watch -p {projectDirectory} run");
        }
    }
}
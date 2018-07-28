using LanguageExt;

namespace Build.Utility
{
    public static class Dotnet
    {
        public static OptionAsync<Unit> Publish(string project, string configuration, string buildDirectory, string verbosity)
        {
            return CommandLine.RunToOption($"dotnet publish {project} --configuration {configuration} --output {buildDirectory}/publish --verbosity {verbosity}");
        }

        public static OptionAsync<Unit> Test(string project, string configuration, string buildDirectory, string verbosity)
        {
            return CommandLine.RunToOption($"dotnet test {project} --configuration {configuration} --output {buildDirectory}/test --results-directory {buildDirectory}/test/results --verbosity {verbosity} --logger trx;logfilename=results.xml");
        }

        public static OptionAsync<Unit> Clean(string project, string configuration, string verbosity)
        {
            return CommandLine.RunToOption($"dotnet clean {project} --configuration {configuration} --verbosity {verbosity}");
        }
    }
}
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Build.Utility;
using static Build.Utility.CommandLine;
using static Build.Utility.Utilities;
using static LanguageExt.Prelude;

namespace Build
{
    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            // q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic]
            var verbosity = "m";
            var configuration = "release";
            var directory = Environment.CurrentDirectory;
            var buildDirectory = $"{directory}/artifacts";
            
            LogStart("Pipeline for MoneyFlow");

            Log($"Building in configuration '{configuration}'");

            Log($"Working directory: {directory}");

            Log($"Dotnet SDK version: {await RunToString("dotnet --version")}");

            return await SomeAsync(unit)
                
                .Bind(_ => Section("Clean", () => 
                    DeleteRecursive(buildDirectory)))
                
                .Bind(_ => Section("Test", () => 
                    RunToSuccess($"dotnet test test/Web --configuration {configuration} --output {buildDirectory}/test --results-directory {buildDirectory}/test/results --verbosity {verbosity} --logger trx;logfilename=results.xml")))
                
                .Bind(_ => Section("Restore", () => 
                    RunToSuccess($"dotnet restore src/Web --verbosity {verbosity}")))
                
                .Bind(_ => Section("Build", () => 
                    RunToSuccess($"dotnet build src/Web --configuration {configuration} --output {buildDirectory}/build --verbosity {verbosity}")))
                
                .Bind(_ => Section("Publish", () => 
                    RunToSuccess($"dotnet publish src/Web --configuration {configuration} --output {buildDirectory}/publish --verbosity {verbosity}")))
                
                .Bind(_ => Section("Zip", () => 
                    ZipDirectory($"{buildDirectory}/publish", $"{buildDirectory}/publish.zip")))
                
                .Match(
                    _ =>
                    {
                        Log();
                        LogSuccess("Build completed successfully");   
                        return 0;
                    }, 
                    () =>
                    {
                        Log();
                        LogError("Build failed");
                        return 1;
                    });
        }
    }
}

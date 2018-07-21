using System;
using System.Threading.Tasks;
using static System.Console;
using static Build.CommandLine;

namespace Build
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            // q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic]
            var configuration = "release";
            var directory = Environment.CurrentDirectory;
            var buildDirectory = $"{directory}/artifacts";
            
            WriteLine("Pipeline for MoneyFlow");

            WriteLine($"Building in configuration '{configuration}'");

            WriteLine($"Working directory: {directory}");

            WriteLine($"Dotnet SDK version:");

            await Run("dotnet --version");
            
            await Run($"dotnet test test/Web --configuration {configuration} --output {buildDirectory}/test --verbosity m");
        }
    }
}

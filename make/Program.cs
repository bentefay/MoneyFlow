using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using Make.Configuration;
using Make.Utility;
using Make.Utility.Commands;
using Make.Utility.Commands.Executables;
using Make.Utility.Extensions;
using McMaster.Extensions.CommandLineUtils;
using static LanguageExt.Prelude;
using static Make.Utility.Utilities;

namespace Make
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var config = new Config();
            var dotnetConfig = new DotnetConfig();
            var parcelConfig = new ParcelConfig();
            var cancel = new CancellationTokenSource();
            var options = new ExecutionOptions(cancellationToken: cancel.Token);

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                LogSection("PRESSED CTRL+C. EXITING...", ConsoleColor.Red);
                cancel.Cancel();
            };

            return new CommandLineApplication
            {
                Name = "make",
                Description = "Build, test and run MoneyFlow",
                UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.StopParsingAndCollect
            }
                .WithExecutableCommand("ui|c", client => ExecuteCommandClient(parcelConfig, client, options), "Execute any command in client working directory")
                .WithExecutableCommand("build|b", build => Build(config, dotnetConfig, parcelConfig), "Build zip of app for production deployment (same as CI build)")
                .WithCommand("run|r", run => run
                    .WithExecutableCommand("ui|c", client => RunClient(config, dotnetConfig, parcelConfig, options))
                    .WithExecutableCommand("server|s", server => RunServer(config, dotnetConfig, options))
                    .WithExecuteShowingHelp(),
                    "Run client or server in local watch mode")
                .WithCommand("test|t", test => test
                    .WithExecutableCommand("server|s", server => TestServer(config, dotnetConfig, options))
                    .WithExecuteShowingHelp(),
                    "Test server in local watch mode")
                .WithExecuteShowingHelp()
                .Execute(args);
        }

        private static Task<int> ExecuteCommandClient(ParcelConfig p, CommandLineApplication client, ExecutionOptions options)
        {
            return Executable.RunAsEither(options.With(workingDirectory: p.Project.ProjectDirectory), client.RemainingArguments.ToArray())
                .ToExitCode();
        }

        private static Task<int> RunClient(Config c, DotnetConfig d, ParcelConfig p, ExecutionOptions options)
        {
            return Do(
                    () => Npm.Install(p.Project.ProjectDirectory, options),
                    () => Parcel.RunDev(p.Project.ProjectDirectory, $"{d.Project.Dir}/wwwroot", $"{c.BuildDir}/ui/cache", p.Verbosity, options))
                .ToExitCode();
        }

        private static Task<int> RunServer(Config c, DotnetConfig d, ExecutionOptions options)
        {
            return Do(
                    () => Dotnet.RunWatch(d.Project.Dir, options))
                .ToExitCode();
        }

        private static Task<int> TestServer(Config c, DotnetConfig d, ExecutionOptions options)
        {
            return Do(
                    () => d.TestProjects.Do(testProject =>
                        Dotnet.TestWatch(testProject.Dir, options)))
                .ToExitCode();
        }

        private static Task<int> Build(Config c, DotnetConfig d, ParcelConfig p)
        {
            return Do(
                    () => LogInfo(c, d).ToAsync(),
                    () => DoSection("Clean",
                        () => DeleteRecursive(c.BuildDir),
                        () => Dotnet.Clean(d.Project.Dir, d.Configuration, d.Verbosity),
                        () => d.TestProjects.Do(testProject =>
                            Dotnet.Clean(testProject.Dir, d.Configuration, d.Verbosity))
                    ),
                    () => DoSection("Test Server",
                        () => d.TestProjects.Do(testProject =>
                            Dotnet.Test(testProject.Dir, d.Configuration, d.Verbosity, $"{c.BuildDir}/test/{testProject.Name}", $"{c.BuildDir}/test/results", testProject.Name))
                    ),
                    () => DoSection("Publish Server",
                        () => Dotnet.Publish(d.Project.Dir, d.Configuration, d.Verbosity, c.PublishDir)
                    ),
                    () => DoSection("Publish Client",
                        () => Npm.Ci(p.Project.ProjectDirectory),
                        () => Parcel.BuildProd(p.Project.ProjectDirectory, $"{c.PublishDir}/wwwroot", $"{c.BuildDir}/ui/cache", p.Verbosity)
                    ),
                    () => DoSection("Zip",
                        () => ZipDirectory(c.PublishDir, c.PublishZipPath)
                    )
                )
                .ToExitCode();
        }

        private static async Task<Either<Error, Unit>> LogInfo(Config c, DotnetConfig d)
        {
            Log($"Working directory: {c.RootDir}");

            Log($"Building in configuration '{d.Configuration}'");

            Log($"Dotnet SDK version: {await Executable.RunAsString("dotnet --version")}");

            Log($"Node version: {await Executable.RunAsString("node --version")}");

            Log($"NPM version: {await Executable.RunAsString("npm --version")}");

            return unit;
        }
    }
}
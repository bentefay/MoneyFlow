using System.Linq;
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

            return new CommandLineApplication
                {
                    Name = "make",
                    Description = "Build, test and run MoneyFlow",
                    ThrowOnUnexpectedArgument = false
                }
                .WithCommand("client|c", client => client.WithExecute(() => ExecuteCommandClient(parcelConfig, client)))
                .WithCommand("build|b", build => build.WithExecute(() => Build(config, dotnetConfig, parcelConfig)))
                .WithCommand("run|r", run => run
                    .WithCommand("client|c", client => client.OnExecute(() => RunClient(config, dotnetConfig, parcelConfig)))
                    .WithCommand("server|s", server => server.OnExecute(() => RunServer(config, dotnetConfig)))
                    .WithExecuteShowingHelp())
                .WithCommand("test|t", test => test
                    .WithCommand("server|s", server => server.OnExecute(() => TestServer(config, dotnetConfig)))
                    .WithExecuteShowingHelp())
                .WithExecuteShowingHelp()
                .Execute(args);
        }

        private static Task<int> ExecuteCommandClient(ParcelConfig p, CommandLineApplication client)
        {
            return Executable.RunToEither(new ExecutionOptions(p.Project.ProjectDirectory), client.RemainingArguments.ToArray())
                .ToExitCode();
        }

        private static Task<int> RunClient(Config c, DotnetConfig d, ParcelConfig p)
        {
            return Do(
                    () => Npm.Install(p.Project.ProjectDirectory),
                    () => Parcel.RunDev(p.Project.ProjectDirectory, $"{d.Project.Dir}/wwwroot", $"{c.BuildDir}/client/cache", p.Verbosity))
                .ToExitCode();
        }

        private static Task<int> RunServer(Config c, DotnetConfig d)
        {
            return Do(
                    () => Dotnet.RunWatch(d.Project.Dir))
                .ToExitCode();
        }

        private static Task<int> TestServer(Config c, DotnetConfig d)
        {
            return Do(
                    () => d.TestProjects.Do(testProject =>
                        Dotnet.TestWatch(testProject.Dir)))
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
                        () => Parcel.BuildProd(p.Project.ProjectDirectory, $"{c.PublishDir}/wwwroot", $"{c.BuildDir}/client/cache", p.Verbosity)
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

            Log($"Dotnet SDK version: {await Executable.RunToString("dotnet --version")}");
            
            Log($"Node version: {await Executable.RunToString("node --version")}");
            
            Log($"NPM version: {await Executable.RunToString("npm --version")}");

            return unit;
        }
    }
}
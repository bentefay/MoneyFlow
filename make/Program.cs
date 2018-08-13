using System.Threading.Tasks;
using LanguageExt;
using Make.Models;
using Make.Utility;
using McMaster.Extensions.CommandLineUtils;
using static LanguageExt.Prelude;
using static Make.Utility.Utilities;

namespace Make
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "make",
                Description = "Build, test and run MoneyFlow"
            };

            app.HelpOption(inherited: true);

            var config = new Config();
            var dotnetConfig = new DotnetConfig();
            var parcelConfig = new ParcelConfig();

            app.Command("build", build => { build.OnExecute(() => Build(config, dotnetConfig, parcelConfig)); });

            app.Command("run", run =>
            {
                run.Command("client", client => { client.OnExecute(() => RunClient(config, parcelConfig)); });
                run.Command("server", server => { server.OnExecute(() => RunServer(config, dotnetConfig)); });
                run.OnExecuteShowHelp();
            });

            app.Command("test", test =>
            {
                test.Command("server", server => { server.OnExecute(() => TestServer(config, dotnetConfig)); });
                test.OnExecuteShowHelp();
            });

            app.OnExecuteShowHelp();

            return app.Execute(args);
        }

        private static Task<int> RunClient(Config c, ParcelConfig p)
        {
            return Do(
                    () => Npm.Install(p.Project.ProjectDirectory),
                    () => Parcel.RunDev(p.Project.ProjectDirectory, p.Verbosity))
                .ToExitCode();
        }

        private static Task<int> RunServer(Config c, DotnetConfig d)
        {
            return Dotnet.Run(d.Project.Dir)
                .ToExitCode();
        }

        private static Task<int> TestServer(Config c, DotnetConfig d)
        {
            return d.TestProjects.ForEach(testProject =>
                    Dotnet.Test(testProject.Dir))
                .ToExitCode();
        }

        private static async Task<int> Build(Config c, DotnetConfig d, ParcelConfig p)
        {
            Log($"Working directory: {c.RootDir}");

            Log($"Building in configuration '{d.Configuration}'");

            Log($"Dotnet SDK version: {await CommandLine.ToString("dotnet --version")}");

            return await Do(
                    () => DoSection("Clean",
                        () => DeleteRecursive(c.BuildDir),
                        () => Dotnet.Clean(d.Project.Dir, d.Configuration, d.Verbosity),
                        () => d.TestProjects.ForEach(testProject =>
                            Dotnet.Clean(testProject.Dir, d.Configuration, d.Verbosity))
                    ),
                    () => DoSection("Test Server",
                        () => d.TestProjects.ForEach(testProject =>
                            Dotnet.Test(testProject.Dir, d.Configuration, d.Verbosity, $"{c.BuildDir}/test/{testProject.Name}", $"{c.BuildDir}/test/results", testProject.Name))
                    ),
                    () => DoSection("Publish Server",
                        () => Dotnet.Publish(d.Project.Dir, d.Configuration, d.Verbosity, c.PublishDir)
                    ),
                    () => DoSection("Publish Client",
                        () => Npm.Install(p.Project.ProjectDirectory),
                        () => Npm.Update(p.Project.ProjectDirectory),
                        () => Parcel.BuildProd(p.Project.ProjectDirectory, $"{c.PublishDir}/wwwroot", $"{c.BuildDir}/client/cache", p.Verbosity)
                    ),
                    () => DoSection("Zip",
                        () => ZipDirectory(c.PublishDir, c.PublishZipPath)
                    )
                )
                .ToExitCode();
        }
    }
}
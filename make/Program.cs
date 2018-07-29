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

            var config = new Config();
            var dotnetConfig = new DotnetConfig();
            var parcelConfig = new ParcelConfig();
            
            app.Command("build", build => { build.OnExecute(() => Build(config, dotnetConfig, parcelConfig)); });
            
            app.Command("run", run =>
            {
                run.Command("client", client => { client.OnExecute(() => RunClient(config, parcelConfig)); });
                run.Command("server", server => { server.OnExecute(() => RunServer(config, dotnetConfig)); });
            });
            
            app.Command("test", test =>
            {
                test.Command("server", server => { server.OnExecute(() => Build(config, dotnetConfig, parcelConfig)); });
            });

            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 1;
            });

            return app.Execute(args);
        }

        public static Task<int> RunClient(Config c, ParcelConfig p)
        {
            return Npm.Update("")
                .Bind(_ => Parcel.Build(p.Project.ProjectDirectory, "--open"))
                .ToExitCode();
        }
        
        public static Task<int> RunServer(Config c, DotnetConfig d)
        {
            return Dotnet.Run(d.Project.Dir)
                .ToExitCode();
        }
        
        public static Task<int> TestServer(Config c, DotnetConfig d)
        {
            return d.TestProjects.ForEach(testProject =>
                    Dotnet.Test(testProject.Dir))
                .ToExitCode();
        }

        private static async Task<int> Build(Config c, DotnetConfig d, ParcelConfig p)
        {
            Log($"Working directory: {c.RootDir}");

            Log($"Building in configuration '{d.Configuration}'");

            Log($"Dotnet SDK version: {await CommandLine.RunToString("dotnet --version")}");

            return await RightAsync<Error, Unit>(Task.FromResult(unit))
                .Bind(_ =>
                    Section("Clean", () =>
                        DeleteRecursive(c.BuildDir)
                            .Bind(unit =>
                                Dotnet.Clean(d.Project.Dir, d.Configuration, d.Verbosity))
                            .Bind(unit => d.TestProjects.ForEach(testProject =>
                                Dotnet.Clean(testProject.Dir, d.Configuration, d.Verbosity)))))
                .Bind(_ =>
                    Section("Test Server", () =>
                        d.TestProjects.ForEach(testProject =>
                            Dotnet.Test(testProject.Dir, d.Configuration, d.Verbosity, $"{c.BuildDir}/test/{testProject.Name}", $"{c.BuildDir}/test/results", testProject.Name))))
                .Bind(_ =>
                    Section("Publish Server", () =>
                        Dotnet.Publish(d.Project.Dir, d.Configuration, d.Verbosity, c.PublishDir)))
                .Bind(_ =>
                    Section("Publish Client", () =>
                        Npm.Update("")
                            .Bind(none => Parcel.Build(p.Project.ProjectDirectory, $"{c.PublishDir}/wwwroot", $"{c.BuildDir}/client/cache"))))
                .Bind(_ =>
                    Section("Zip",
                        () => ZipDirectory(c.PublishDir, c.PublishZipPath)))
                .ToExitCode();
        }
    }
}
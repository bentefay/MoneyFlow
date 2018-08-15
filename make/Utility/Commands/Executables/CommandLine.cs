using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LanguageExt;
using Make.Utility.Extensions;

namespace Make.Utility.Commands.Executables
{
    public class CommandLine
    {
        private CommandLine(string exe, string arguments)
        {
            Exe = exe;
            Arguments = arguments;
        }

        public string Exe { get; }
        public string Arguments { get; }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Arguments) ? Exe : string.Join(" ", Exe, Arguments);
        }

        public static EitherAsync<Error, CommandLine> Resolve(params string[] command)
        {
            var commandTokens = command
                .SelectMany(argument => argument.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .ToList();

            if (!commandTokens.Any())
                return Error.Create($"Could not resolve empty command");
            
            var exe = commandTokens
                .First()
                .Trim();
            
            var arguments = commandTokens
                .Skip(1)
                .Select(a => a.Trim())
                .Join(" ");

            return ResolveExePath(exe)
                .Map(exeAbsolutePath => new CommandLine($"\"{exeAbsolutePath}\"", arguments));
        }

        private static EitherAsync<Error, string> ResolveExePath(string exe)
        {
            try
            {
                var favouredExtensions = FavouredExtensions().Concat(new[] {""});
                var exePaths = Path.HasExtension(exe)
                    ? new[] {exe}
                    : favouredExtensions.Select(extension =>
                        string.IsNullOrEmpty(extension) ? exe : Path.ChangeExtension(exe, extension));

                var basePathsToSearch = Path.IsPathRooted(exe) ? new[] {""} : new[] {""}.Concat(GetEnvironmentPaths());

                var exeAbsolutePaths = basePathsToSearch
                    .SelectMany(basePath => exePaths.Select(exePath => string.IsNullOrEmpty(basePath) ? exePath : Path.Combine(basePath, exePath)));
                    
                var exeAbsolutePath = exeAbsolutePaths
                    .FirstOrDefault(File.Exists);

                return string.IsNullOrEmpty(exeAbsolutePath)
                    ? Prelude.LeftAsync<Error, string>(Task.FromResult(CouldNotFindExe(exe, exeAbsolutePaths)))
                    : exeAbsolutePath;
            }
            catch (Exception e)
            {
                return Error.Create($"Error while trying to resolve location of '{exe}'", e);
            }
        }

        private static Error CouldNotFindExe(string exe, IEnumerable<string> exeAbsolutePaths)
        {
            return Error.Create($"Could not find location of exe '{exe}'. " +
                                $"Searched the following locations:{Environment.NewLine}{string.Join(Environment.NewLine, exeAbsolutePaths)}");
        }

        private static string[] GetEnvironmentPaths()
        {
            var pathEnv = Environment.GetEnvironmentVariable("PATH");
            return pathEnv == null ? new string[0] : pathEnv.Split(';');
        }

        private static string[] FavouredExtensions()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? new[] {".exe", ".cmd"} : new string[] { };
        }

    }
}
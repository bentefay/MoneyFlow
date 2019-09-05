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
    public class ExecutionArguments
    {
        private ExecutionArguments(string exe, string arguments)
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

        public string ToString(ExecutionOptions options)
        {
            return !string.IsNullOrWhiteSpace(options.WorkingDirectory) ?
                $"[{options.WorkingDirectory}] {ToString()}" :
                $"{ToString()}";
        }

        public static EitherAsync<Error, ExecutionArguments> Resolve(params string[] command)
        {
            var commandTokens = command
                .SelectMany(argument => argument.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .ToList();

            if (!commandTokens.Any())
                return Error.Create($"Could not resolve empty command");

            var executable = commandTokens
                .First()
                .Trim();

            var arguments = commandTokens
                .Skip(1)
                .Select(a => a.Trim())
                .Join(" ");

            return ResolveExecutablePath(executable)
                .Map(exeAbsolutePath => exeAbsolutePath.Contains(" ") ? $"\"{exeAbsolutePath}\"" : exeAbsolutePath)
                .Map(exeAbsolutePath => new ExecutionArguments(exeAbsolutePath, arguments));
        }

        private static EitherAsync<Error, string> ResolveExecutablePath(string executable)
        {
            try
            {
                var defaultExtensions = GetDefaultExtensions().Concat(new[] {""});
                var exePaths = Path.HasExtension(executable)
                    ? new[] {executable}
                    : defaultExtensions.Select(extension =>
                        string.IsNullOrEmpty(extension) ? executable : Path.ChangeExtension(executable, extension));

                var basePathsToSearch = Path.IsPathRooted(executable) ?
                    new[] {""} :
                    new[] {""}.Concat(GetEnvironmentPaths());

                var executableAbsolutePaths = basePathsToSearch
                    .SelectMany(basePath => exePaths.Select(exePath => string.IsNullOrEmpty(basePath) ? exePath : Path.Combine(basePath, exePath)))
                    .ToList();

                var executableAbsolutePath = executableAbsolutePaths
                    .FirstOrDefault(File.Exists);

                return string.IsNullOrEmpty(executableAbsolutePath)
                    ? Prelude.LeftAsync<Error, string>(Task.FromResult(CouldNotFindExe(executable, executableAbsolutePaths)))
                    : executableAbsolutePath;
            }
            catch (Exception e)
            {
                return Error.Create($"Error while trying to resolve location of '{executable}'", e);
            }
        }

        private static Error CouldNotFindExe(string exe, IEnumerable<string> exeAbsolutePaths)
        {
            return Error.Create($"Could not find location of exe '{exe}'. " +
                                $"Searched the following locations:{Environment.NewLine}{string.Join(Environment.NewLine, exeAbsolutePaths)}");
        }

        private static IReadOnlyList<string> GetEnvironmentPaths()
        {
            var pathEnv = Environment.GetEnvironmentVariable("PATH");
            return pathEnv == null ? new string[0] : pathEnv.Split(Path.PathSeparator);
        }

        private static IReadOnlyList<string> GetDefaultExtensions() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                new[] {".exe", ".cmd"} :
                new [] { ".sh" };
    }
}
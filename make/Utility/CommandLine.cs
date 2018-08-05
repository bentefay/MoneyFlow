using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using static Make.Utility.Utilities;

namespace Make.Utility
{
    public static class CommandLine
    {
        public static async Task<string> ToString(params string[] command)
        {
            return await RunExpanded(command, redirectStreams: true)
                .Match(
                    result => string.Join(Environment.NewLine, result.Output),
                    _ => "");
        }

        public static EitherAsync<Error, Unit> RunToOption(params string[] command)
        {
            return ResolveCommand(command)
                .Bind(c =>
                {
                    Log($"{c.Exe} {c.Arguments}");
                    return Run(c);
                })
                .Where(result => result.ExitCode == 0)
                .Map(_ => unit);
        }

        private static EitherAsync<Error, (string Exe, string Arguments)> ResolveCommand(params string[] command)
        {
            var commandTokens = command.SelectMany(argument => argument.Split(' ', StringSplitOptions.RemoveEmptyEntries)).ToList();
            var exe = commandTokens.First().Trim();
            var arguments = commandTokens.Skip(1).Select(a => a.Trim()).Join(" ");

            return ResolveExePath(exe)
                .Map(exeAbsolutePath => ($"\"{exeAbsolutePath}\"", arguments));
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
                    ? LeftAsync<Error, string>(Task.FromResult(CouldNotFindExe(exe, exeAbsolutePaths)))
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

        private static EitherAsync<Error, CommandLineResult> RunExpanded(string[] command, bool redirectStreams = false, CancellationToken? cancellationToken = null)
        {
            return ResolveCommand(command)
                .Bind(c => Run(c, redirectStreams, cancellationToken));
        }

        // Reference: https://github.com/jamesmanning/RunProcessAsTask/blob/master/src/RunProcessAsTask/ProcessEx.cs#L27

        private static async Task<Either<Error, CommandLineResult>> Run((string Exe, string Arguments) command, bool redirectStreams = false, CancellationToken? cancellationToken = null)
        {
            cancellationToken = cancellationToken ?? CancellationToken.None;

            var processStartInfo = new ProcessStartInfo(command.Exe, command.Arguments)
            {
                UseShellExecute = false,
                RedirectStandardOutput = redirectStreams,
                RedirectStandardError = redirectStreams
            };

            var taskCompletionSource = new TaskCompletionSource<Either<Error, CommandLineResult>>();

            var process = new Process
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true
            };

            var (standardOutput, standardError) = redirectStreams ? GetStreams(process) : CreateEmptyStreams();

            process.Exited += async (sender, args) =>
            {
                taskCompletionSource.TrySetResult(
                    new CommandLineResult(
                        process.ExitCode,
                        await standardOutput.ConfigureAwait(false),
                        await standardError.ConfigureAwait(false)
                    )
                );
            };

            void Cancel()
            {
                taskCompletionSource.TrySetCanceled();
                try
                {
                    if (!process.HasExited)
                        process.Kill();
                }
                catch (InvalidOperationException)
                {
                }
            }

            using (cancellationToken.Value.Register(Cancel))
            {
                cancellationToken.Value.ThrowIfCancellationRequested();

                try
                {
                    if (!process.Start())
                    {
                        taskCompletionSource.TrySetResult(Error.Create($"Could not start command '{ToString(command)}'"));
                    }
                    else if (redirectStreams)
                    {
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();
                    }
                }
                catch (Exception e)
                {
                    taskCompletionSource.TrySetResult(Error.Create($"Error while running command '{ToString(command)}': {e.Message}", e));
                }

                return await taskCompletionSource.Task.ConfigureAwait(false);
            }
        }

        private static readonly Task<IReadOnlyList<string>> _emptyStream = Task.FromResult<IReadOnlyList<string>>(new List<string>());

        private static (Task<IReadOnlyList<string>>, Task<IReadOnlyList<string>>) CreateEmptyStreams()
        {
            return (_emptyStream, _emptyStream);
        }

        private static (Task<IReadOnlyList<string>> StandardOutput, Task<IReadOnlyList<string>> StandardError) GetStreams(Process process)
        {
            var standardOutputResults = new TaskCompletionSource<IReadOnlyList<string>>();
            var standardOutput = new List<string>();
            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                    standardOutput.Add(args.Data);
                else
                    standardOutputResults.SetResult(standardOutput);
            };

            var standardErrorResults = new TaskCompletionSource<IReadOnlyList<string>>();
            var standardError = new List<string>();
            process.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                    standardError.Add(args.Data);
                else
                    standardErrorResults.SetResult(standardError);
            };

            return (standardOutputResults.Task, standardErrorResults.Task);
        }

        private static string ToString((string Exe, string Arguments) command)
        {
            return string.IsNullOrEmpty(command.Arguments) ? command.Exe : string.Join(" ", command.Exe, command.Arguments);
        }
    }
}
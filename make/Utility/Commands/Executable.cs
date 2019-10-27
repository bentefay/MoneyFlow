using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using LanguageExt;
using Make.Utility.Commands.Executables;
using Make.Utility.Extensions;

namespace Make.Utility.Commands
{
    public static class Executable
    {
        public static Task<string> RunAsString(params string[] command) =>
            RunAsString(null, command);

        public static async Task<string> RunAsString(ExecutionOptions? options, params string[] command)
        {
            options ??= new ExecutionOptions();

            return await Run(command, options.With(redirectStreams: true))
                .Match(
                    result => string.Join(Environment.NewLine, result.Output),
                    _ => "");
        }

        public static EitherAsync<Error, Unit> RunAsEither(params string[] command) => RunAsEither(null, command);

        public static EitherAsync<Error, Unit> RunAsEither(ExecutionOptions? options, params string[] command)
        {
            options ??= new ExecutionOptions();

            return ExecutionArguments.Resolve(command)
                .Bind(executionArguments =>
                {
                    Utilities.Log();
                    Utilities.Log(executionArguments.ToString(options));
                    return Run(executionArguments, options.With(redirectStreams: false));
                })
                .Bind(result =>
                    result.ExitCode == 0 ?
                        Prelude.Right<Error, Unit>(Prelude.unit) :
                        Error.Create($"'{result.ExecutionArguments.ToString(options)}'\n\n=> exit code '{result.ExitCode}'"));
        }

        public static EitherAsync<Error, ExecutionResult> Run(
            string[] command,
            ExecutionOptions options)
        {
            return ExecutionArguments.Resolve(command)
                .Bind(c => Run(c, options));
        }

        // Reference: https://github.com/jamesmanning/RunProcessAsTask/blob/master/src/RunProcessAsTask/ProcessEx.cs#L27

        public static async Task<Either<Error, ExecutionResult>> Run(
            ExecutionArguments executionArguments,
            ExecutionOptions? options = null)
        {
            options ??= new ExecutionOptions();

            var processStartInfo = new ProcessStartInfo(executionArguments.Exe, executionArguments.Arguments)
            {
                UseShellExecute = false,
                RedirectStandardOutput = options.RedirectStreams,
                RedirectStandardError = options.RedirectStreams,
                WorkingDirectory = options.WorkingDirectory
            };

            var taskCompletionSource = new TaskCompletionSource<Either<Error, ExecutionResult>>();

            var process = new Process
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true
            };

            var (standardOutput, standardError) = options.RedirectStreams ? GetStreams(process) : CreateEmptyStreams();

            process.Exited += async (sender, args) =>
            {
                var standardOutputValue = await standardOutput.ConfigureAwait(false);
                var standardErrorValue = await standardError.ConfigureAwait(false);

                taskCompletionSource.TrySetResult(
                    new ExecutionResult(
                        executionArguments,
                        options,
                        process.ExitCode,
                        standardOutputValue,
                        standardErrorValue
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

            using (options.CancellationToken.Register(Cancel))
            {
                options.CancellationToken.ThrowIfCancellationRequested();

                try
                {
                    if (!process.Start())
                    {
                        taskCompletionSource.TrySetResult(
                            Error.Create($"Could not start command '{executionArguments}' from working directory '{options.WorkingDirectory}'"));
                    }
                    else if (options.RedirectStreams)
                    {
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();
                    }
                }
                catch (Exception e)
                {
                    taskCompletionSource.TrySetResult(
                        Error.Create($"Error while running command '{executionArguments}' from working directory '{options.WorkingDirectory}': {e.Message}", e));
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
    }
}
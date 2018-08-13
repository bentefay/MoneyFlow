﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            return await Run(command, new CommandLineOptions(redirectStreams: true))
                .Match(
                    result => string.Join(Environment.NewLine, result.Output),
                    _ => "");
        }

        public static EitherAsync<Error, Unit> ToEither(params string[] command) => ToEither(null, command);

        public static EitherAsync<Error, Unit> ToEither(CommandLineOptions options, params string[] command)
        {
            options = options ?? new CommandLineOptions();
            
            return Command.Resolve(command)
                .Bind(c =>
                {
                    Log();
                    Log(!string.IsNullOrWhiteSpace(options.WorkingDirectory) ? $"[{options.WorkingDirectory}] {c}" : $"{c}");
                    return Run(c, options.With(redirectStreams: false));
                })
                .Where(result => result.ExitCode == 0)
                .Map(_ => unit);
        }

        private static EitherAsync<Error, CommandLineResult> Run(
            string[] command,             
            CommandLineOptions options)
        {
            return Command.Resolve(command)
                .Bind(c => Run(c, options));
        }

        // Reference: https://github.com/jamesmanning/RunProcessAsTask/blob/master/src/RunProcessAsTask/ProcessEx.cs#L27

        private static async Task<Either<Error, CommandLineResult>> Run(
            Command command, 
            CommandLineOptions options = null)
        {
            options = options ?? new CommandLineOptions();
            
            var processStartInfo = new ProcessStartInfo(command.Exe, command.Arguments)
            {
                UseShellExecute = false,
                RedirectStandardOutput = options.RedirectStreams,
                RedirectStandardError = options.RedirectStreams,
                WorkingDirectory = options.WorkingDirectory
            };

            var taskCompletionSource = new TaskCompletionSource<Either<Error, CommandLineResult>>();

            var process = new Process
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true
            };

            var (standardOutput, standardError) = options.RedirectStreams ? GetStreams(process) : CreateEmptyStreams();

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

            using (options.CancellationToken.Register(Cancel))
            {
                options.CancellationToken.ThrowIfCancellationRequested();

                try
                {
                    if (!process.Start())
                    {
                        taskCompletionSource.TrySetResult(Error.Create($"Could not start command '{command}'"));
                    }
                    else if (options.RedirectStreams)
                    {
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();
                    }
                }
                catch (Exception e)
                {
                    taskCompletionSource.TrySetResult(Error.Create($"Error while running command '{command}': {e.Message}", e));
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
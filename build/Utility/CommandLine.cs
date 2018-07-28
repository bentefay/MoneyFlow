using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Build.Utility
{
    public static class CommandLine
    {      
        public static async Task<string> RunToString(params string[] arguments)
        {
            return await Run(arguments, redirectStreams: true)
                .ToAsync()
                .Match(
                    result => string.Join(Environment.NewLine, result.Output) + string.Join(Environment.NewLine, result.Error),
                    () => "");
        }

        public static OptionAsync<Unit> RunToOption(params string[] arguments)
        {
            return Run(arguments, redirectStreams: false)
                .ToAsync()
                .Where(result => result.ExitCode == 0)
                .Map(_ => unit);
        }
        
        public static Task<Option<CommandLineResult>> Run(params string[] arguments)
        {
            return Run(arguments, redirectStreams: false);
        }
        
        // Reference: https://github.com/jamesmanning/RunProcessAsTask/blob/master/src/RunProcessAsTask/ProcessEx.cs#L27
        
        public static async Task<Option<CommandLineResult>> Run(IEnumerable<string> arguments, bool redirectStreams = false, CancellationToken? cancellationToken = null)
        {
            cancellationToken = cancellationToken ?? CancellationToken.None;

            var argumentTokens = arguments.SelectMany(argument => argument.Split(' ')).ToList();
            
            var processStartInfo = new ProcessStartInfo(argumentTokens.First(), argumentTokens.Skip(1).Join(" "))
            {
                UseShellExecute = false,
                RedirectStandardOutput = redirectStreams,
                RedirectStandardError = redirectStreams
            };

            var taskCompletionSource = new TaskCompletionSource<Option<CommandLineResult>>();
           
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
                catch (InvalidOperationException) { }
            }

            using (cancellationToken.Value.Register(Cancel)) 
            {

                cancellationToken.Value.ThrowIfCancellationRequested();

                try
                {
                    if (!process.Start())
                    {
                        taskCompletionSource.TrySetResult(Prelude.None);
                    }
                    else if (redirectStreams)
                    {
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error while running command '{argumentTokens.Join(" ")}': {e.Message}");
                    taskCompletionSource.TrySetResult(Prelude.None);
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
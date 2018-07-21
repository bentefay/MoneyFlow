using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;

namespace Build
{
    public static class CommandLine
    {
        public static Task<Option<CommandLineResult>> Run(string command)
        {
            var tokens = command.Split(' ');
            return Run(tokens.First(), tokens.Skip(1).Join(" "), CancellationToken.None);
        }
        
        public static Task<Option<CommandLineResult>> Run(string name, string arguments) =>
            Run(name, arguments, CancellationToken.None);
        
        public static async Task<Option<CommandLineResult>> Run(string name, string arguments, CancellationToken cancellationToken)
        {
            var processStartInfo = new ProcessStartInfo(name, arguments)
            {
                UseShellExecute = false,
                RedirectStandardOutput = false,
                RedirectStandardError = false
            };

            var taskCompletionSource = new TaskCompletionSource<Option<CommandLineResult>>();

            var process = new Process
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true
            };

            process.Exited += (sender, args) =>
            {
                taskCompletionSource.TrySetResult(
                    new CommandLineResult(
                        process.ExitCode
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

            using (cancellationToken.Register(Cancel)) 
            {

                cancellationToken.ThrowIfCancellationRequested();

                if (process.Start() == false)
                    taskCompletionSource.TrySetResult(Prelude.None);

                return await taskCompletionSource.Task.ConfigureAwait(false);
            }
        }
    }
}
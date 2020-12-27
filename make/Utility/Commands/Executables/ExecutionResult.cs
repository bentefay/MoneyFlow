using System.Collections.Generic;

namespace Make.Utility.Commands.Executables
{
    public class ExecutionResult
    {
        public ExecutionArguments ExecutionArguments { get; }
        public ExecutionOptions Options { get; }
        public int ExitCode { get; }
        public IReadOnlyList<string> Output { get; }
        public IReadOnlyList<string> Error { get; }

        public ExecutionResult(
            ExecutionArguments executionArguments,
            ExecutionOptions options,
            int exitCode,
            IReadOnlyList<string> output,
            IReadOnlyList<string> error)
        {
            ExecutionArguments = executionArguments;
            Options = options;
            ExitCode = exitCode;
            Output = output;
            Error = error;
        }
    }
}
using System.Collections.Generic;

namespace Make.Utility.Commands.Executables
{
    public class ExecutionResult
    {
        public int ExitCode { get; }
        public IReadOnlyList<string> Output { get; }
        public IReadOnlyList<string> Error { get; }

        public ExecutionResult(int exitCode, IReadOnlyList<string> output, IReadOnlyList<string> error)
        {
            ExitCode = exitCode;
            Output = output;
            Error = error;
        }
    }
}
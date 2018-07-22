using System.Collections.Generic;

namespace Build.Utility
{
    public class CommandLineResult
    {
        public int ExitCode { get; }
        public IReadOnlyList<string> Output { get; }
        public IReadOnlyList<string> Error { get; }

        public CommandLineResult(int exitCode, IReadOnlyList<string> output, IReadOnlyList<string> error)
        {
            ExitCode = exitCode;
            Output = output;
            Error = error;
        }
    }
}
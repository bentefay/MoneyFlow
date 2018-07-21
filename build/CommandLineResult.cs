namespace Build
{
    public class CommandLineResult
    {
        public CommandLineResult(int exitCode)
        {
            ExitCode = exitCode;
        }

        public int ExitCode { get; }
    }
}
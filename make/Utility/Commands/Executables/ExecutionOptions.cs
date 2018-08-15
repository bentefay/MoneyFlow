using System.Threading;

namespace Make.Utility.Commands.Executables
{
    public class ExecutionOptions
    {
        public ExecutionOptions(string workingDirectory = null, bool redirectStreams = false, CancellationToken? cancellationToken = null)
        {
            WorkingDirectory = workingDirectory;
            RedirectStreams = redirectStreams;
            CancellationToken = cancellationToken ?? CancellationToken.None;
        }

        public ExecutionOptions With(string workingDirectory = null, bool? redirectStreams = null, CancellationToken? cancellationToken = null)
        {
            return new ExecutionOptions(
                workingDirectory ?? WorkingDirectory,
                redirectStreams ?? RedirectStreams,
                cancellationToken ?? CancellationToken);
        }

        public string WorkingDirectory { get; }
        public bool RedirectStreams { get; }
        public CancellationToken CancellationToken { get; }
    }
}
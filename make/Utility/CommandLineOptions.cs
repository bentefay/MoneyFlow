using System.Threading;

namespace Make.Utility
{
    public class CommandLineOptions
    {
        public CommandLineOptions(string workingDirectory = null, bool redirectStreams = false, CancellationToken? cancellationToken = null)
        {
            WorkingDirectory = workingDirectory;
            RedirectStreams = redirectStreams;
            CancellationToken = cancellationToken ?? CancellationToken.None;
        }

        public CommandLineOptions With(string workingDirectory = null, bool? redirectStreams = null, CancellationToken? cancellationToken = null)
        {
            return new CommandLineOptions(
                workingDirectory ?? WorkingDirectory,
                redirectStreams ?? RedirectStreams,
                cancellationToken ?? CancellationToken);
        }

        public string WorkingDirectory { get; }
        public bool RedirectStreams { get; }
        public CancellationToken CancellationToken { get; }
    }
}
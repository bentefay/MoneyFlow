using McMaster.Extensions.CommandLineUtils;

namespace Make.Utility
{
    internal static class CommandLineApplicationExtensions
    {
        public static void OnExecuteShowHelp(this CommandLineApplication app)
        {
            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 1;
            });
        }
    }
}
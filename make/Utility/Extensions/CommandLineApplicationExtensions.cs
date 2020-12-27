using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Make.Utility.Extensions
{
    internal static class CommandLineApplicationExtensions
    {
        public static void OnExecuteShowHelp(this CommandLineApplication app)
        {
            app.OnExecute(() =>
            {
                app.ShowHelp(usePager: false);
                return 1;
            });
        }

        public static CommandLineApplication WithExecute(this CommandLineApplication app, Func<Task<int>> f)
        {
            app.OnExecuteAsync(_ => f());
            return app;
        }

        public static CommandLineApplication WithExecuteShowingHelp(this CommandLineApplication app)
        {
            app.OnExecuteShowHelp();
            return app;
        }

        public static CommandLineApplication WithCommand(this CommandLineApplication app, string name, Action<CommandLineApplication> f, string? description = null)
        {
            var names = name
                .Split('|', StringSplitOptions.RemoveEmptyEntries);

            names
                .Iter((i, nameAlias) =>
                {
                    var command = app.Command(nameAlias, f);
                    if (i == 0)
                    {
                        command.Description = names.Length > 1 ?
                            $"[{string.Join("|", names)}] {description}" :
                            description;
                    }
                    else
                    {
                        command.ShowInHelpText = false;
                    }
                    command.UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.StopParsingAndCollect;
                });
            return app;
        }

        public static CommandLineApplication WithExecutableCommand(this CommandLineApplication app, string name, Func<CommandLineApplication, Task<int>> f, string? description = null)
        {
            return WithCommand(app, name, application => application.WithExecute(() => f(application)), description);
        }

    }
}
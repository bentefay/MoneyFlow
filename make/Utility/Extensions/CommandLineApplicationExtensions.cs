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
                app.ShowHelp();
                return 1;
            });
        }

        public static CommandLineApplication WithExecute(this CommandLineApplication app, Func<Task<int>> f)
        {
            app.OnExecute(f);
            return app;
        }
        
        public static CommandLineApplication WithExecuteShowingHelp(this CommandLineApplication app)
        {
            app.OnExecuteShowHelp();
            return app;
        } 
        
        public static CommandLineApplication WithCommand(this CommandLineApplication app, string name, Action<CommandLineApplication> f)
        {
            name
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Iter(nameAlias =>
                {
                    var command = app.Command(nameAlias, f);
                    command.ThrowOnUnexpectedArgument = false;
                });
            return app;
        }
        
        public static CommandLineApplication WithExecutableCommand(this CommandLineApplication app, string name, Func<CommandLineApplication, Task<int>> f)
        {
            return WithCommand(app, name, application => application.WithExecute(() => f(application)));
        }

    }
}
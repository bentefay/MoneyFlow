using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Make.Utility
{
    public static class Utilities
    {
        public static EitherAsync<Error, Unit> Do(params Func<EitherAsync<Error, Unit>>[] functions)
        {
            return functions.Aggregate(RightAsync<Error, Unit>(Task.FromResult(unit)), (acc, f) => acc.Bind(_ => f()));
        }

        public static EitherAsync<Error, Unit> DoSection(string title, params Func<EitherAsync<Error, Unit>>[] functions)
        {
            LogSection(title);
            return Do(functions);
        }

        public static void LogSection(string text, ConsoleColor color = ConsoleColor.DarkCyan)
        {
            Log();
            Log(new string('=', text.Length), color);
            Log(text, color);
            Log(new string('=', text.Length), color);
            Log();
        }

        public static void LogSuccess(string text) => Log(text, ConsoleColor.Green);

        public static void LogError(string text) => Log(text, ConsoleColor.Red);

        public static void Log(string text = "", ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public static EitherAsync<Error, Unit> DeleteRecursive(string directoryPath)
        {
            try
            {
                if (Directory.Exists(directoryPath))
                {
                    Directory.Delete(directoryPath, recursive: true);
                }
                return unit;
            }
            catch (Exception e)
            {
                return Error.Create($"Failed to recusively delete directory '{directoryPath}': {e.Message}", e);
            }
        }

        public static EitherAsync<Error, Unit> ZipDirectory(string source, string destination)
        {
            try
            {
                Log($"Zipping '{source}' to '{destination}'");
                ZipFile.CreateFromDirectory(source, destination);
                return unit;
            }
            catch (Exception e)
            {
                return Error.Create($"Failed to zip '{source}' to '{destination}': {e.Message}", e);
            }
        }
    }
}
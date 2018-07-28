using System;
using System.IO;
using System.IO.Compression;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Build.Utility
{
    public static class Utilities
    {       
        public static T Section<T>(string title, Func<T> func)
        {
            LogSection(title);
            return func();
        }
        
        public static void LogStart(string text)
        {
            Log();
            Log(new string('+', text.Length), ConsoleColor.Green);
            Log(text, ConsoleColor.Green);
            Log(new string('+', text.Length), ConsoleColor.Green);
            Log();
        }
        
        public static void LogSection(string text)
        {
            Log();
            Log(new string('=', text.Length), ConsoleColor.DarkCyan);
            Log(text, ConsoleColor.DarkCyan);
            Log(new string('=', text.Length), ConsoleColor.DarkCyan);
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
        
        public static OptionAsync<Unit> DeleteRecursive(string directoryPath)
        {
            try
            {
                if (Directory.Exists(directoryPath))
                {
                    Directory.Delete(directoryPath, recursive: true);
                }
                return SomeAsync(unit);
            }
            catch (Exception e)
            {
                LogError($"Failed to recusively delete directory '{directoryPath}': {e.Message}");
                return None;
            }
        }

        public static Option<Unit> ZipDirectory(string source, string destination)
        {
            try
            {
                ZipFile.CreateFromDirectory(source, destination);
                return Some(unit);
            }
            catch (Exception e)
            {
                LogError($"Failed to zip '{source}' to '{destination}': {e.Message}");
                return None;
            }
        }
    }
}
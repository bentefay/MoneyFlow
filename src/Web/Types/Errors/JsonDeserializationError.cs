using System;
using System.Linq;
using Web.Utils.Extensions;

namespace Web.Types.Errors
{
    public class JsonDeserializationError : IErrorWithException, IDeserializeUserErrors, IParseAuthorizationErrors
    {
        public Type Type { get; }
        public string Json { get; }
        public string Path { get; }
        public int LineNumber { get; }
        public int LinePosition { get; }
        public Exception Exception { get; }

        public JsonDeserializationError(Type type, string json, string path, int lineNumber, int linePosition, Exception exception)
        {
            Type = type;
            Json = json;
            Path = path;
            LineNumber = lineNumber;
            LinePosition = linePosition;
            Exception = exception;
        }

        public string GetDescription() => GetDescription(Type, Path, Json, LineNumber, LinePosition, Exception);

        public static string GetDescription(Type type, string path, string json, int lineNumber, int linePosition, Exception exception)
        {
            var annotatedJson = AnnotateJson(json, lineNumber, linePosition);
            return $"Deserialization error for object of type '{type?.Name}' at path '{path}' ({lineNumber},{linePosition}):\n{exception.Message}\n\n{annotatedJson}'";
        }

        private static string AnnotateJson(string json, int lineNumber, int linePosition)
        {
            var lines = json.Split(Environment.NewLine).ToList();
            
            if (lineNumber < 1 || lineNumber > lines.Count)
                return json;

            var lineToAnnotate = lines[lineNumber - 1];

            if (linePosition < 1 || linePosition > lineToAnnotate.Length)
            {
                lines.Insert(linePosition, new string('^', lineToAnnotate.Length));
            }
            else
            {
                lines.Insert(linePosition, new string(' ', linePosition - 1) + "^");                
            }

            var contextLines = 20;
            
            return lines
                .Skip(Math.Max(0, linePosition - contextLines))
                .Take(2 * contextLines)
                .Join(Environment.NewLine);
        } 
    }
}
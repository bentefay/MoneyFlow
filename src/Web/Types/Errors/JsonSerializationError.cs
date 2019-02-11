using System;

namespace Web.Types.Errors
{
    public class JsonSerializationError : IError
    {
        public Type Type { get; }
        public object Value { get; }
        public string Path { get; }
        public string Message { get; }

        public JsonSerializationError(Type type, object value, string path, string message)
        {
            Type = type;
            Value = value;
            Path = path;
            Message = message;
        }

        public string GetDescription() => 
            $"Serialization error for object of type '{Type?.Name}' at path '{Path}':\n{Message}";
    }
}
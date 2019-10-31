using System;

namespace Web.Types.Errors
{
    public class JsonSerializationError : IError, ISerializeVaultIndexErrors
    {
        public Type Type { get; }
        public object? Value { get; }
        public string Path { get; }
        public Exception Exception { get; }

        public JsonSerializationError(Type type, object? value, string path, Exception exception)
        {
            Type = type;
            Value = value;
            Path = path;
            Exception = exception;
        }

        public string GetDescription() => 
            $"Serialization error for object of type '{Type?.Name}' at path '{Path}':\n{Exception.Message}";
    }
}
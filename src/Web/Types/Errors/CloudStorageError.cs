using System;

namespace Web.Types.Errors
{
    internal class CloudStorageError : IError
    {
        private readonly Exception _exception;
        private readonly string _action;

        public CloudStorageError(Exception exception, string action)
        {
            _exception = exception;
            _action = action;
        }

        public string GetDescription() => $"Error while {_action}: {_exception.Message}";
    }
}
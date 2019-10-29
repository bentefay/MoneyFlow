using System;

namespace Web.Types.Errors
{
    public class HashPasswordError : IError
    {
        private readonly Exception _error;

        public HashPasswordError(Exception error)
        {
            _error = error;
        }

        public string GetDescription() => $"Failed to hash password: {_error.Message}";
    }
}
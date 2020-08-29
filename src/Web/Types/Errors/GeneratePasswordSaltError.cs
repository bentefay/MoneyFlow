using System;

namespace Web.Types.Errors
{
    public class GeneratePasswordSaltError : IError, IBuilderUserErrors
    {
        private readonly Exception _error;

        public GeneratePasswordSaltError(Exception error)
        {
            _error = error;
        }

        public string GetDescription() => $"Failed to generate salt: {_error.Message}";
    }
}
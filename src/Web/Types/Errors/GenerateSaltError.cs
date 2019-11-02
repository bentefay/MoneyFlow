using System;

namespace Web.Types.Errors
{
    public class GenerateSaltError : IError, ICreateVaultIndexErrors
    {
        private readonly Exception _error;

        public GenerateSaltError(Exception error)
        {
            _error = error;
        }

        public string GetDescription() => $"Failed to generate salt: {_error.Message}";
    }
}
using System;

namespace Web.Types.Errors
{
    public class HashPasswordError : IErrorWithException, IAssertVaultAccessErrors, ICreateVaultIndexErrors
    {
        public Exception Exception { get; }

        public HashPasswordError(Exception exception)
        {
            Exception = exception;
        }

        public string GetDescription() => $"Failed to hash password: {Exception.Message}";
    }
}
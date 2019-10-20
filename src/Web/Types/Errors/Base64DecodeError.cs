using System;

namespace Web.Types.Errors
{
    public class Base64DecodeError : IError
    {
        private readonly string _base64;
        private readonly Exception _error;

        public Base64DecodeError(string base64, Exception error)
        {
            _base64 = base64;
            _error = error;
        }
        
        public string GetDescription() => $"Could not decode base64 string '{_base64}' as it was invalid: {_error.Message}";
    }
}
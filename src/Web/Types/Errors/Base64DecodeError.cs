using System;

namespace Web.Types.Errors
{
    public class Base64DecodeError : IErrorWithException, IParseAuthorizationErrors
    {
        public string Base64 { get; }
        public Exception Exception { get; }

        public Base64DecodeError(string base64, Exception error)
        {
            Base64 = base64;
            Exception = error;
        }
        
        public string GetDescription() => $"Could not decode base64 string '{Base64}' as it was invalid: {Exception.Message}";
    }
}
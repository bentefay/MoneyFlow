using System;
using LanguageExt;

namespace Make.Utility
{
    public class Error
    {
        public string Message { get; }
        public Option<Exception> Exception { get; }

        private Error(string message, Option<Exception> exception)
        {
            Message = message;
            Exception = exception;
        }

        public static Error Create(string reason, Exception? e = null)
        {
            return new Error(reason, Prelude.Optional(e!));
        }
    }
}
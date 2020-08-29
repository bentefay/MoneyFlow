using System;
using Web.Types.Values;

namespace Web.Types.Errors
{
    public class EmailIncorrectError : IError, IAssertVaultAccessErrors
    {
        public Email ExpectedEmail { get; }
        public Email ActualEmail { get; }

        public EmailIncorrectError(Email expectedEmail, Email actualEmail)
        {
            ExpectedEmail = expectedEmail;
            ActualEmail = actualEmail;
        }

        public string GetDescription() => $"'{ActualEmail}' was incorrect (expected '{ExpectedEmail}')";
    }
    
    
}
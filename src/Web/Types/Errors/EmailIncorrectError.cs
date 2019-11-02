using System;
using Web.Types.Values;

namespace Web.Types.Errors
{
    public class EmailIncorrectError : IError, IAssertVaultAccessErrors
    {
        public Email Email { get; }

        public EmailIncorrectError(Email email)
        {
            Email = email;
        }

        public string GetDescription() => $" '{Email}' was incorrect";
    }
    
    
}
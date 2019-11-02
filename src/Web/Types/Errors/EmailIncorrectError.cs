using System;
using Web.Types.Values;

namespace Web.Types.Errors
{
    public class EmailIncorrectError : IError, IAssertVaultAccessErrors
    {
        private readonly Email _email;

        public EmailIncorrectError(Email email)
        {
            _email = email;
        }

        public string GetDescription() => $" '{_email}' was incorrect";
    }
    
    
}
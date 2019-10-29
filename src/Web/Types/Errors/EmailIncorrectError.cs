using System;

namespace Web.Types.Errors
{
    public class EmailIncorrectError : IError
    {
        private readonly Email _email;

        public EmailIncorrectError(Email email)
        {
            _email = email;
        }

        public string GetDescription() => $"Email '{_email}' was incorrect";
    }
}
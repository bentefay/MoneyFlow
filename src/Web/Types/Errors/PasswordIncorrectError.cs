namespace Web.Types.Errors
{
    public class PasswordIncorrectError : IError
    {
        private readonly Email _email;

        public PasswordIncorrectError(Email email)
        {
            _email = email;
        }

        public string GetDescription() => $"Password was incorrect for email '{_email}'";
    }
}
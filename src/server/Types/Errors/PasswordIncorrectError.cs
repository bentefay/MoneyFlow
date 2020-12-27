using Web.Types.Values;

namespace Web.Types.Errors
{
    public class PasswordIncorrectError : IAssertVaultAccessErrors
    {
        public Email Email { get; }

        public PasswordIncorrectError(Email email)
        {
            Email = email;
        }

        public string GetDescription() => $"Password was incorrect for email '{Email}'";
    }
}
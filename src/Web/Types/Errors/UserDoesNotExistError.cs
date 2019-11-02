using Web.Types.Values;

namespace Web.Types.Errors
{
    public class UserDoesNotExistError : IAssertVaultAccessErrors, IUpdateVaultErrors
    {
        private readonly Email _email;

        public UserDoesNotExistError(Email email)
        {
            _email = email;
        }

        public string GetDescription() => $"Vault does not exist for '{_email}'";
    }
}
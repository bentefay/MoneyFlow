using Web.Types.Values;

namespace Web.Types.Errors
{
    public class UserDoesNotExistError : IAssertVaultAccessErrors, IUpdateVaultErrors
    {
        public Email Email { get; }

        public UserDoesNotExistError(Email email)
        {
            Email = email;
        }

        public string GetDescription() => $"Vault does not exist for '{Email}'";
    }
}
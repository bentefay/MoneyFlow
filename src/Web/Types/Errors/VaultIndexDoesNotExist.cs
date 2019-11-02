using Web.Types.Values;

namespace Web.Types.Errors
{
    public class VaultIndexDoesNotExist : IAssertVaultAccessErrors, IUpdateVaultErrors
    {
        public Email Email { get; }

        public VaultIndexDoesNotExist(Email email)
        {
            Email = email;
        }

        public string GetDescription() => $"Vault does not exist for '{Email}'";
    }
}
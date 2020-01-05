using Web.Types.Values;

namespace Web.Types.Errors
{
    public class VaultDoesNotExistError : IAssertVaultAccessErrors
    {
        public UserId UserId { get; }
        public Email Email { get; }

        public VaultDoesNotExistError(Email email, UserId userId)
        {
            UserId = userId;
            Email = email;
        }

        public string GetDescription() => $"Vault does not exist for '{Email}' (user id {UserId})";
    }
}
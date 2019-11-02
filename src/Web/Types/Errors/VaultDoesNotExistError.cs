using Web.Types.Values;

namespace Web.Types.Errors
{
    public class VaultDoesNotExistError : IAssertVaultAccessErrors
    {
        private readonly UserId _userId;
        private readonly Email _email;

        public VaultDoesNotExistError(Email email, UserId userId)
        {
            _userId = userId;
            _email = email;
        }

        public string GetDescription() => $"Vault does not exist for '{_email}' (user id {_userId})";
    }
}
using Web.Types.Values;

namespace Web.Types.Errors
{
    public class UserIdMismatchError : IError, IAssertVaultAccessErrors
    {
        private readonly Email _email;
        private readonly UserId _vaultUserId;
        private readonly UserId _vaultIndexUserId;

        public UserIdMismatchError(Email email, UserId vaultUserId, UserId vaultIndexUserId)
        {
            _email = email;
            _vaultUserId = vaultUserId;
            _vaultIndexUserId = vaultIndexUserId;
        }

        public string GetDescription() => $"For '{_email}' there was a mismatch between the vault user id {_vaultUserId} and the vault index user id {_vaultIndexUserId}";
    }
}
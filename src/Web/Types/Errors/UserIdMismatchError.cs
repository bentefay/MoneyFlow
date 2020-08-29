using Web.Types.Values;

namespace Web.Types.Errors
{
    public class UserIdMismatchError : IError, IAssertVaultAccessErrors
    {
        private readonly Email _email;
        private readonly UserId _vaultUserId;
        private readonly UserId _userId;

        public UserIdMismatchError(Email email, UserId vaultUserId, UserId userId)
        {
            _email = email;
            _vaultUserId = vaultUserId;
            _userId = userId;
        }

        public string GetDescription() => $"For '{_email}' there was a mismatch between the vault user id {_vaultUserId} and the user id {_userId}";
    }
}
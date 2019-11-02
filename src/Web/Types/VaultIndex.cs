using LanguageExt;
using Web.Types.Values;

namespace Web.Types
{
    public class VaultIndex : Record<VaultIndex>
    {
        public VaultIndex(UserId userId, Email email, PasswordSalt passwordSalt, DoubleHashedPassword password)
        {
            UserId = userId;
            Email = email;
            PasswordSalt = passwordSalt;
            Password = password;
        }

        public UserId UserId { get; }
        public Email Email { get; }
        public PasswordSalt PasswordSalt { get; }
        public DoubleHashedPassword Password { get; }
    }

    public class TaggedVaultIndex : VaultIndex
    {
        public StorageETag ETag { get; }

        public TaggedVaultIndex(UserId userId, Email email, PasswordSalt passwordSalt, DoubleHashedPassword password, StorageETag eTag) : 
            base(userId, email, passwordSalt, password)
        {
            ETag = eTag;
        }
    }
}
using LanguageExt;

namespace Web.Types
{
    public class NewVaultIndex : Record<NewVaultIndex>
    {
        public NewVaultIndex(Email email, PasswordSalt passwordSalt, DoubleHashedPassword password)
        {
            Email = email;
            PasswordSalt = passwordSalt;
            Password = password;
        }

        public Email Email { get; }
        public PasswordSalt PasswordSalt { get; }
        public DoubleHashedPassword Password { get; }
    }
    
    public class VaultIndex : NewVaultIndex
    {
        public VaultIndex(Email email, PasswordSalt passwordSalt, DoubleHashedPassword password, StorageETag eTag) : base(email, passwordSalt, password)
        {
            ETag = eTag;
        }

        public StorageETag ETag { get; }
    }
}
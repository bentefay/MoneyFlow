namespace Web.Types
{
    public class VaultIndex
    {
        public VaultIndex(Email email, PasswordSalt passwordSalt, DoubleHashedPassword password, StorageConcurrencyLock concurrencyLock)
        {
            Email = email;
            PasswordSalt = passwordSalt;
            Password = password;
            ConcurrencyLock = concurrencyLock;
        }

        public Email Email { get; }
        public PasswordSalt PasswordSalt { get; }
        public DoubleHashedPassword Password { get; }
        public StorageConcurrencyLock ConcurrencyLock { get; }
    }
}
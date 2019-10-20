namespace Web.Types
{
    public class WrappedVault
    {
        public ulong Version { get; }
        public Base64EncryptedVault Vault { get; }

        public WrappedVault(ulong version, Base64EncryptedVault vault)
        {
            Version = version;
            Vault = vault;
        }
    }
}
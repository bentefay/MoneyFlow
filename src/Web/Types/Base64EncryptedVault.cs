namespace Web.Types
{
    public class Base64EncryptedVault : TinyType<Base64EncryptedVault, string>
    {
        public Base64EncryptedVault(string value) : base(value)
        {
        }
    }
}
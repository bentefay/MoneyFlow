using System.IO;

namespace Web.Types.Values
{
    public class Base64EncodedEncryptedVault
    {
        public string Value { get; }

        public Base64EncodedEncryptedVault(string value)
        {
            Value = value;
        }
    }
}
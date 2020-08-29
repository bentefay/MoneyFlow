using LanguageExt;
using Web.Types.Values;

namespace Web.Types
{
    public class Vault : Record<Vault>
    {
        public Vault(UserId userId, Base64EncodedEncryptedVault content)
        {
            UserId = userId;
            Content = content;
        }

        public UserId UserId { get; }
        public Base64EncodedEncryptedVault Content { get; }

        public StorageETag? GetETag() => this is TaggedVault t ? t.ETag : null;
    }

    public class TaggedVault : Vault
    {
        public StorageETag ETag { get; }

        public TaggedVault(UserId userId, Base64EncodedEncryptedVault content, StorageETag eTag) : base(userId, content)
        {
            ETag = eTag;
        }
    }
}
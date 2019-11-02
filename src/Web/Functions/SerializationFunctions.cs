using LanguageExt;
using Web.Types;
using Web.Types.Dtos.Storage;
using Web.Types.Errors;
using Web.Types.Values;
using Web.Utils.Extensions;
using Web.Utils.Serialization.Serializers;

namespace Web.Functions
{
    public static class SerializationFunctions
    {
        public static Either<ISerializeVaultIndexErrors, string> SerializeVaultIndex(VaultIndex vaultIndex)
        {
            var dto = MappingFunctions.ToDto(vaultIndex);

            return JsonFunctions.Serialize(dto, ApiSerializers.Instance).Left(Cast.To<ISerializeVaultIndexErrors>());
        }

        public static Either<IDeserializeVaultIndexErrors, TaggedVaultIndex> DeserializeVaultIndex(string json, StorageETag eTag)
        {
            return
                from dto in JsonFunctions.Deserialize<VaultIndexDto>(json, ApiSerializers.Instance).Left(Cast.To<IDeserializeVaultIndexErrors>())
                from vaultIndex in MappingFunctions.FromDto(dto, eTag)
                select vaultIndex;
        }
        
        public static Either<ISerializeVaultErrors, string> SerializeVault(Vault vault)
        {
            return vault.Content.Value;
        }

        public static Either<IDeserializeVaultErrors, TaggedVault> DeserializeVault(string data, UserId userId, StorageETag eTag)
        {
            var dto = new Base64EncodedEncryptedVault(data);
            return new TaggedVault(userId, dto, eTag);
        }
    }
}
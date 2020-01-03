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
        public static Either<ISerializeUserErrors, string> SerializeUser(User user)
        {
            var dto = MappingFunctions.ToDto(user);

            return JsonFunctions.Serialize(dto, ApiSerializers.Instance).Left(Cast.To<ISerializeUserErrors>());
        }

        public static Either<IDeserializeUserErrors, TaggedUser> DeserializeUser(string json, StorageETag eTag)
        {
            return
                from dto in JsonFunctions.Deserialize<UserDto>(json, ApiSerializers.Instance).Left(Cast.To<IDeserializeUserErrors>())
                from user in MappingFunctions.FromDto(dto, eTag)
                select user;
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
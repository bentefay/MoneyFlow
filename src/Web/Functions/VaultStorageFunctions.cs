using LanguageExt;
using Web.Utils.Extensions;
using Web.Types;
using Web.Types.Dtos;
using Web.Types.Errors;
using Web.Utils.Serialization.Serializers;

namespace Web.Functions
{
    public class VaultStorageFunctions
    {
        private static readonly string _container = "vaults";
        private static string GetIndexPath(Email email) => $"{email.Value}/index";
        
        private static EitherAsync<ISaveNewVaultIndexErrors, Unit> SaveNewVaultIndex(NewVaultIndex vaultIndex, StorageConnectionString connectionString)
        {
            return
                from blob in StorageFunctions.GetBlob(_container, GetIndexPath(vaultIndex.Email), connectionString).ToAsync().Left(Cast.To<ISaveNewVaultIndexErrors>())
                from json in SerializeVaultIndex(vaultIndex).ToAsync().Left(Cast.To<ISaveNewVaultIndexErrors>())
                from _ in StorageFunctions.SetBlobText(blob, json).Left(Cast.To<ISaveNewVaultIndexErrors>())
                select Prelude.unit;
        }
        
        private static EitherAsync<IUpdateVaultIndexErrors, Unit> UpdateVaultIndex(VaultIndex vaultIndex, StorageConnectionString connectionString)
        {
            return
                from blob in StorageFunctions.GetBlob(_container, GetIndexPath(vaultIndex.Email), connectionString).ToAsync().Left(Cast.To<IUpdateVaultIndexErrors>())
                from json in SerializeVaultIndex(vaultIndex).ToAsync().Left(Cast.To<IUpdateVaultIndexErrors>())
                from _ in StorageFunctions.SetBlobText(blob, json, vaultIndex.ETag).Left(Cast.To<IUpdateVaultIndexErrors>())
                select Prelude.unit;
        }


        public static EitherAsync<ILoadVaultIndexErrors, Option<VaultIndex>> LoadVaultIndex(Email email, StorageConnectionString connectionString)
        {
            return
                    from blob in StorageFunctions.GetBlob(_container, GetIndexPath(email), connectionString).ToAsync().Left(Cast.To<ILoadVaultIndexErrors>())
                    from maybeJson in StorageFunctions.GetBlobText(blob).Left(Cast.To<ILoadVaultIndexErrors>())
                    from vaultIndex in maybeJson
                        .Map(json => DeserializeVaultIndex(json.Text, json.ETag))
                        .Sequence()
                        .ToAsync()
                        .Left(Cast.To<ILoadVaultIndexErrors>())
                    select vaultIndex;
        }

        private static Either<ISerializeVaultIndexErrors, string> SerializeVaultIndex(NewVaultIndex vaultIndex)
        {
            var dto = new VaultIndexDto(
                email: vaultIndex.Email.Value,
                passwordSalt: vaultIndex.PasswordSalt.Value,
                password:  vaultIndex.Password.Value);

            return JsonFunctions.Serialize(dto, ApiSerializers.Instance).Left(Cast.To<ISerializeVaultIndexErrors>());
        }

        private static Either<IDeserializeVaultIndexErrors, VaultIndex> DeserializeVaultIndex(string json, StorageETag eTag)
        {
            return
                from dto in JsonFunctions.Deserialize<VaultIndexDto>(json, ApiSerializers.Instance).Left(Cast.To<IDeserializeVaultIndexErrors>())
                from email in Email.Create(dto.Email).Left(Cast.To<IDeserializeVaultIndexErrors>())
                let salt = new PasswordSalt(dto.PasswordSalt)
                from password in DoubleHashedPassword.Create(dto.Password).Left(Cast.To<IDeserializeVaultIndexErrors>())
                select new VaultIndex(email, salt, password, eTag);
        }
    }
}
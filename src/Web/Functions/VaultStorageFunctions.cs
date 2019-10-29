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
        
        private static EitherAsync<IError, Unit> SaveNewVaultIndex(NewVaultIndex vaultIndex, StorageConnectionString connectionString)
        {
            return
                from blob in StorageFunctions.GetBlob(_container, GetIndexPath(vaultIndex.Email), connectionString).ToAsync()
                from json in SerializeVaultIndex(vaultIndex).ToAsync()
                from _ in StorageFunctions.SetBlobText(blob, json)
                select Prelude.unit;
        }
        
        private static EitherAsync<IError, Unit> UpdateVaultIndex(VaultIndex vaultIndex, StorageConnectionString connectionString)
        {
            return
                from blob in StorageFunctions.GetBlob(_container, GetIndexPath(vaultIndex.Email), connectionString).ToAsync()
                from json in SerializeVaultIndex(vaultIndex).ToAsync()
                from _ in StorageFunctions.SetBlobText(blob, json, vaultIndex.ETag)
                select Prelude.unit;
        }

        public static EitherAsync<IError, Option<VaultIndex>> LoadVaultIndex(Email email, StorageConnectionString connectionString)
        {
            return
                    from blob in StorageFunctions.GetBlob(_container, GetIndexPath(email), connectionString).ToAsync()
                    from maybeJson in StorageFunctions.GetBlobText(blob)
                    from vaultIndex in maybeJson
                        .Map(json => DeserializeVaultIndex(json.Text, json.ETag))
                        .Sequence()
                        .ToAsync()
                    select vaultIndex;
        }

        private static Either<IError, string> SerializeVaultIndex(NewVaultIndex vaultIndex)
        {
            var dto = new VaultIndexDto(
                email: vaultIndex.Email.Value,
                passwordSalt: vaultIndex.PasswordSalt.Value,
                password:  vaultIndex.Password.Value);

            return JsonFunctions.Serialize(dto, ApiSerializers.Instance).Left(Cast.To<IError>());
        }

        private static Either<IError, VaultIndex> DeserializeVaultIndex(string json, StorageETag eTag)
        {
            return
                from dto in JsonFunctions.Deserialize<VaultIndexDto>(json, ApiSerializers.Instance).Left(Cast.To<IError>())
                from email in Email.Create(dto.Email).Left(Cast.To<IError>())
                let salt = new PasswordSalt(dto.PasswordSalt)
                from password in DoubleHashedPassword.Create(dto.Password).Left(Cast.To<IError>())
                select new VaultIndex(email, salt, password, eTag);
        }
    }
}
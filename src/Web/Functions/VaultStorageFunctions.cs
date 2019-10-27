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
        private static EitherAsync<IError, Unit> CreateVault(VaultIndex vaultIndex, StorageConnectionString connectionString)
        {
            return
                from blob in BlobFunctions.Get("vaults", $"{vaultIndex.Email.Value}/index", connectionString)
                from _ in BlobFunctions.Exists(blob)
                    .Bind(exists => exists ?
                        Prelude.LeftAsync<IError, Unit>(new VaultAlreadyExists(vaultIndex.Email)) :
                        Prelude.unit)
                from json in SerializeVaultIndex(vaultIndex).ToAsync()
                from __ in BlobFunctions.SetText(blob, json)
                select Prelude.unit;
        }

        public static EitherAsync<IError, Option<VaultIndex>> GetVaultIndex(Email email, StorageConnectionString connectionString)
        {
            return
                    from blob in BlobFunctions.Get("vaults", $"{email.Value}/index", connectionString)
                    from maybeJson in BlobFunctions.GetText(blob)
                    from vaultIndex in maybeJson
                        .Map(json => DeserializeVaultIndex(json, blob.Properties.ETag))
                        .Sequence()
                        .ToAsync()
                    select vaultIndex;
        }

        private static Either<IError, string> SerializeVaultIndex(VaultIndex vaultIndex)
        {
            var dto = new VaultIndexDto(
                email: vaultIndex.Email.Value,
                passwordSalt: vaultIndex.PasswordSalt.Value,
                password:  vaultIndex.Password.Value);

            return JsonFunctions.Serialize(dto, ApiSerializers.Instance).Left(Cast.To<IError>());
        }

        private static Either<IError, VaultIndex> DeserializeVaultIndex(string json, string etag)
        {
            return
                from dto in JsonFunctions.Deserialize<VaultIndexDto>(json, ApiSerializers.Instance).Left(Cast.To<IError>())
                from email in Email.Create(dto.Email).Left(Cast.To<IError>())
                let salt = new PasswordSalt(dto.PasswordSalt)
                let concurrencyLock = new StorageConcurrencyLock(etag)
                from password in DoubleHashedPassword.Create(dto.Password).Left(Cast.To<IError>())
                select new VaultIndex(email, salt, password, concurrencyLock);
        }
    }
}
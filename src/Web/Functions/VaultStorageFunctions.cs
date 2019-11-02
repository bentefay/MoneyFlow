using LanguageExt;
using Web.Utils.Extensions;
using Web.Types;
using Web.Types.Errors;
using Web.Types.Values;

namespace Web.Functions
{
    public static class VaultStorageFunctions
    {
        private const string Container = "vaults";
        private static string GetIndexPath(Email email) => $"{email.Value}/index";
        private static string GetVaultPath(UserId userId) => $"{userId.Value}/vault";
        
        public static EitherAsync<ISaveNewVaultIndexErrors, Unit> CreateVaultIndex(VaultIndex vaultIndex, StorageConnectionString connectionString)
        {
            return
                from blob in StorageFunctions.GetBlob(Container, GetIndexPath(vaultIndex.Email), connectionString).ToAsync().Left(Cast.To<ISaveNewVaultIndexErrors>())
                from json in SerializationFunctions.SerializeVaultIndex(vaultIndex).ToAsync().Left(Cast.To<ISaveNewVaultIndexErrors>())
                from _ in StorageFunctions.SetBlobText(blob, json).Left(Cast.To<ISaveNewVaultIndexErrors>())
                select Prelude.unit;
        }
        
        public static EitherAsync<IUpdateVaultIndexErrors, Unit> UpdateVaultIndex(TaggedVaultIndex vaultIndex, StorageConnectionString connectionString)
        {
            return
                from blob in StorageFunctions.GetBlob(Container, GetIndexPath(vaultIndex.Email), connectionString).ToAsync().Left(Cast.To<IUpdateVaultIndexErrors>())
                from json in SerializationFunctions.SerializeVaultIndex(vaultIndex).ToAsync().Left(Cast.To<IUpdateVaultIndexErrors>())
                from _ in StorageFunctions.SetBlobText(blob, json, vaultIndex.ETag).Left(Cast.To<IUpdateVaultIndexErrors>())
                select Prelude.unit;
        }

        public static EitherAsync<ILoadVaultIndexErrors, Option<TaggedVaultIndex>> LoadVaultIndex(Email email, StorageConnectionString connectionString)
        {
            return
                from blob in StorageFunctions.GetBlob(Container, GetIndexPath(email), connectionString).ToAsync().Left(Cast.To<ILoadVaultIndexErrors>())
                from maybeJson in StorageFunctions.GetBlobText(blob).Left(Cast.To<ILoadVaultIndexErrors>())
                from vaultIndex in maybeJson
                    .Map(json => SerializationFunctions.DeserializeVaultIndex(json.Text, json.ETag))
                    .Sequence()
                    .ToAsync()
                    .Left(Cast.To<ILoadVaultIndexErrors>())
                select vaultIndex;
        }
        
        
        public static EitherAsync<ILoadVaultErrors, Option<TaggedVault>> LoadVault(UserId userId, StorageConnectionString connectionString)
        {
            return
                from blob in StorageFunctions.GetBlob(Container, GetVaultPath(userId), connectionString).ToAsync().Left(Cast.To<ILoadVaultErrors>())
                from maybeData in StorageFunctions.GetBlobText(blob).Left(Cast.To<ILoadVaultErrors>())
                from vault in maybeData
                    .Map(data => SerializationFunctions.DeserializeVault(data.Text, userId, data.ETag))
                    .Sequence()
                    .ToAsync()
                    .Left(Cast.To<ILoadVaultErrors>())
                select vault;
        }
        
        public static EitherAsync<IUpdateVaultErrors, Unit> UpdateVault(Vault vault, StorageConnectionString connectionString)
        {
            return
                from blob in StorageFunctions.GetBlob(Container, GetVaultPath(vault.UserId), connectionString).ToAsync().Left(Cast.To<IUpdateVaultErrors>())
                from data in SerializationFunctions.SerializeVault(vault).ToAsync().Left(Cast.To<IUpdateVaultErrors>())
                from _ in StorageFunctions.SetBlobText(blob, data, vault.GetETag()).Left(Cast.To<IUpdateVaultErrors>())
                select Prelude.unit;
        }


    }
}
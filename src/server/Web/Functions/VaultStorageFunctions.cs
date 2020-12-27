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
        private static string GetVaultPath(UserId userId) => $"{userId.Value}/vault";

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
using System;
using System.IO;
using LanguageExt;
using Web.Utils.Extensions;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Web.Types;
using Web.Types.Dtos;
using Web.Types.Errors;
using Web.Utils.Extensions;
using Web.Utils.Serialization.Serializers;

namespace Web.Functions
{
    public class VaultStorageFunctions
    {
        private static EitherAsync<IError, VaultIndex> CreateVault(VaultIndex vaultIndex, StorageConnectionString connectionString)
        {
            return PreludeExt.CreateEitherAsync(async () =>
            {
                if (!CloudStorageAccount.TryParse(connectionString.Value, out var account))
                    return new InvalidCloudStorageConnectionString();

                var path = $"{vaultIndex.Email.Value}/index";

                try
                {
                    var serviceClient = account.CreateCloudBlobClient();
                    var container = serviceClient.GetContainerReference("vaults");
                    var cloudBlob = (CloudBlockBlob) await container.GetBlobReferenceFromServerAsync(path);
                    if (cloudBlob.Exists())
                    {
                        var json = await cloudBlob.DownloadTextAsync();
                        return (
                            from dto in JsonFunctions.Deserialize<VaultIndexDto>(json, ApiSerializers.Instance).Left(Cast.To<IError>())
                            from email in Email.Create(dto.Email).Left(Cast.To<IError>())
                            let salt = new PasswordSalt(dto.PasswordSalt)
                            let etag = new StorageConcurrencyLock(cloudBlob.Properties.ETag)
                            from password in DoubleHashedPassword.Create(dto.Password).Left(Cast.To<IError>())
                            select new VaultIndex(email, salt, password, etag)
                        );
                    }
                    else
                    {
                        return Prelude.Right<VaultIndex?>(null);
                    }
                }
                catch (Exception e)
                {
                    return new CloudStorageError(e, $"reading vault summary from {path}");
                }
            });
        }

        private static EitherAsync<IError, Option<VaultIndex>> GetVaultIndex(Email email, StorageConnectionString connectionString)
        {
            return
                    from cloudBlob in StorageFunctions.GetBlob("vault", $"{email.Value}/index", connectionString)
                    from maybeJson in StorageFunctions.GetBlobText(cloudBlob)
                    from vaultIndex in maybeJson
                        .Map(json => CreateVaultIndex(json, cloudBlob.Properties.ETag))
                        .Sequence()
                        .ToAsync()
                    select vaultIndex;
        }

        private static Either<IError, VaultIndex> CreateVaultIndex(string json, string etag)
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
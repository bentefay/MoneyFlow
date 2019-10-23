using System;
using LanguageExt;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Web.Types;
using Web.Types.Errors;
using Web.Utils.Extensions;

namespace Web.Functions
{
    public static class StorageFunctions
    {
        public static EitherAsync<IError, CloudBlockBlob> GetBlob(string containerName, string path, StorageConnectionString connectionString)
        {
            return PreludeExt.CreateEitherAsync<IError, CloudBlockBlob>(async () =>
            {
                if (!CloudStorageAccount.TryParse(connectionString.Value, out var account))
                    return new InvalidCloudStorageConnectionString();

                try
                {
                    var serviceClient = account.CreateCloudBlobClient();
                    var container = serviceClient.GetContainerReference(containerName);
                    var cloudBlob = (CloudBlockBlob) await container.GetBlobReferenceFromServerAsync(path);
                    return cloudBlob;
                }
                catch (Exception e)
                {
                    return new CloudStorageError(e, $"reading cloud blob from {path}");
                }
            });
        }

        public static EitherAsync<IError, Option<string>> GetBlobText(CloudBlockBlob blob)
        {
            return PreludeExt.CreateEitherAsync(
                async () => {
                    try
                    {
                        var text = await blob.DownloadTextAsync();
                        return Prelude.Some(text);
                    }
                    catch (Exception e)
                    {
                        return Prelude.Left<IError, Option<string>>(new CloudStorageError(e, $"reading cloud blob text from {blob.StorageUri}"));
                    }
                });
        }
        
        public static EitherAsync<IError, BlobLeaseId> AcquireBlobLease(CloudBlockBlob blob)
        {
            var leaseTime = TimeSpan.FromSeconds(1);
            return PreludeExt.CreateEitherAsync<IError, BlobLeaseId>(
                async () => {
                    try
                    {
                        return new BlobLeaseId(await blob.AcquireLeaseAsync(leaseTime));
                    }
                    catch (Exception e)
                    {
                        return new CloudStorageError(e, $"acquiring blob lease {blob.StorageUri}");
                    }
                });
        }
        
        public static EitherAsync<IError, Unit> AcquireBlobLease(CloudBlockBlob blob, BlobLeaseId leaseId)
        {
            return PreludeExt.CreateEitherAsync<IError, Unit>(
                async () => {
                    try
                    {
                        await blob.ReleaseLeaseAsync(AccessCondition.GenerateLeaseCondition(leaseId.Value));
                        return Prelude.unit;
                    }
                    catch (Exception e)
                    {
                        return new CloudStorageError(e, $"releasing blob lease {blob.StorageUri}");
                    }
                });
        }
    }
}
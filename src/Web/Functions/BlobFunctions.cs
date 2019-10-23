using System;
using LanguageExt;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Web.Types;
using Web.Types.Errors;
using Web.Utils.Extensions;

namespace Web.Functions
{
    public static class BlobFunctions
    {
        public static EitherAsync<IError, CloudBlockBlob> Get(string containerName, string path, StorageConnectionString connectionString)
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
                    return new CloudStorageError(e, $"reading blob from {path}");
                }
            });
        }

        public static EitherAsync<IError, Option<string>> GetText(CloudBlockBlob blob)
        {
            return PreludeExt.CreateEitherAsync<IError, Option<string>>(
                async () => {
                    try
                    {
                        var text = await blob.DownloadTextAsync();
                        return Prelude.Some(text);
                    }
                    catch (Exception e)
                    {
                        return new CloudStorageError(e, $"reading text from blob {blob.StorageUri}");
                    }
                });
        }

        public static EitherAsync<IError, Unit> SetText(CloudBlockBlob blob, string text, AccessCondition? accessCondition = null)
        {
            return PreludeExt.CreateEitherAsync<IError, Unit>(
                async () => {
                    try
                    {
                        await blob.UploadTextAsync(text, null, accessCondition, null, null);
                        return Prelude.unit;
                    }
                    catch (Exception e)
                    {
                        return new CloudStorageError(e, $"reading text from blob {blob.StorageUri}");
                    }
                });
        }

        public static EitherAsync<IError, bool> Exists(CloudBlockBlob blob)
        {
            return PreludeExt.CreateEitherAsync<IError, bool>(
                async () => {
                    try
                    {
                        return await blob.ExistsAsync();
                    }
                    catch (Exception e)
                    {
                        return new CloudStorageError(e, $"checking if blob exists {blob.StorageUri}");
                    }
                });
        }

        public static EitherAsync<IError, BlobLeaseId> AcquireLease(CloudBlockBlob blob)
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
                        return new CloudStorageError(e, $"acquiring lease on blob {blob.StorageUri}");
                    }
                });
        }

        public static EitherAsync<IError, Unit> ReleaseLease(CloudBlockBlob blob, BlobLeaseId leaseId)
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
                        return new CloudStorageError(e, $"releasing lease {leaseId.Value} on blob {blob.StorageUri}");
                    }
                });
        }
    }
}
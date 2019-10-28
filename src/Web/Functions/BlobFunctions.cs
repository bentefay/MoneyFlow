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
            if (!CloudStorageAccount.TryParse(connectionString.Value, out var account))
                return new InvalidCloudStorageConnectionString();

            try
            {
                var serviceClient = account.CreateCloudBlobClient();
                var container = serviceClient.GetContainerReference(containerName);
                var cloudBlob = container.GetBlockBlobReference(path);
                return cloudBlob;
            }
            catch (Exception e)
            {
                return new CloudStorageError(e, $"reading blob from {path}");
            }
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

        public static EitherAsync<IError, Unit> SetText(CloudBlockBlob blob, string text, StorageConcurrencyLock? concurrencyLock = null)
        {
            return PreludeExt.CreateEitherAsync<IError, Unit>(
                async () => {
                    try
                    {
                        var accessCondition = concurrencyLock == null ? AccessCondition.GenerateIfNotExistsCondition() : AccessCondition.GenerateIfMatchCondition(concurrencyLock.Value);
                        await blob.UploadTextAsync(text, null, accessCondition, null, null);
                        return Prelude.unit;
                    }
                    catch (Exception e)
                    {
                        return new CloudStorageError(e, $"reading text from blob {blob.StorageUri}");
                    }
                });
        }
    }
}
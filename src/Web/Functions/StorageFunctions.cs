using System;
using LanguageExt;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Blob.Protocol;
using Microsoft.Azure.Storage.Shared.Protocol;
using Web.Types;
using Web.Types.Errors;
using Web.Utils.Extensions;

namespace Web.Functions
{
    public static class StorageFunctions
    {
        public static Either<IError, CloudBlockBlob> GetBlob(string containerName, string path, StorageConnectionString connectionString)
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

        public static EitherAsync<IError, Option<StorageText>> GetBlobText(CloudBlockBlob blob)
        {
            return PreludeExt.CreateEitherAsync<IError, Option<StorageText>>(
                async () => {
                    try
                    {
                        var text = await blob.DownloadTextAsync();
                        return StorageETag.Create(blob.Properties.ETag)
                            .Left(Cast.To<IError>())
                            .Map(etag => Prelude.Some(new StorageText(text, etag)));
                    }
                    catch (StorageException e) when (e.RequestInformation.ErrorCode == BlobErrorCodeStrings.BlobNotFound)
                    {
                        return Option<StorageText>.None;
                    }
                    catch (Exception e)
                    {
                        return new CloudStorageError(e, $"reading text from blob {blob.StorageUri}");
                    }
                });
        }

        public static EitherAsync<IError, Unit> SetBlobText(CloudBlockBlob blob, string text, StorageETag? eTag = null)
        {
            return PreludeExt.CreateEitherAsync<IError, Unit>(
                async () => {
                    try
                    {
                        var accessCondition = eTag == null ? 
                            AccessCondition.GenerateIfNotExistsCondition() : 
                            AccessCondition.GenerateIfMatchCondition(eTag.Value);

                        await blob.UploadTextAsync(text, null, accessCondition, null, null);

                        return Prelude.unit;
                    }
                    catch (StorageException e) when (
                        e.RequestInformation.ErrorCode == BlobErrorCodeStrings.BlobAlreadyExists || 
                        e.RequestInformation.ErrorCode == StorageErrorCodeStrings.ConditionNotMet)
                    {
                        return e.RequestInformation.ErrorCode == BlobErrorCodeStrings.BlobAlreadyExists;
                    }
                    catch (Exception e)
                    {
                        return new CloudStorageError(e, $"writing text from blob {blob.StorageUri}");
                    }
                });
        }
    }
}
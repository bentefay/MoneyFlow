using System;
using System.IO;
using LanguageExt;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Blob.Protocol;
using Microsoft.Azure.Storage.Shared.Protocol;
using Web.Types;
using Web.Types.Errors;
using Web.Types.Values;
using Web.Utils.Extensions;

namespace Web.Functions
{
    public static class StorageFunctions
    {
        public static Either<IGetBlobErrors, CloudBlockBlob> GetBlob(string containerName, string path, StorageConnectionString connectionString)
        {
            if (!CloudStorageAccount.TryParse(connectionString.Value, out var account))
                return new MalformedCloudStorageConnectionString();

            try
            {
                var serviceClient = account.CreateCloudBlobClient();
                var container = serviceClient.GetContainerReference(containerName);
                var cloudBlob = container.GetBlockBlobReference(path);
                return cloudBlob;
            }
            catch (Exception e)
            {
                return new GeneralStorageError(e, $"reading blob from {path}");
            }
        }

        public static EitherAsync<IGetBlobTextErrors, Option<StorageText>> GetBlobText(CloudBlockBlob blob)
        {
            return PreludeExt.CreateEitherAsync<IGetBlobTextErrors, Option<StorageText>>(
                async () => {
                    try
                    {
                        var text = await blob.DownloadTextAsync();
                        return StorageETag.Create(blob.Properties.ETag)
                            .Left(Cast.To<IGetBlobTextErrors>())
                            .Map(etag => Prelude.Some(new StorageText(text, etag)));
                    }
                    catch (StorageException e) when (e.RequestInformation.ErrorCode == BlobErrorCodeStrings.BlobNotFound)
                    {
                        return Option<StorageText>.None;
                    }
                    catch (Exception e)
                    {
                        return new GeneralStorageError(e, $"reading text from blob {blob.StorageUri}");
                    }
                });
        }
        
        public static EitherAsync<ISetBlobTextErrors, Unit> SetBlobText(CloudBlockBlob blob, string text, StorageETag? eTag = null)
        {
            return PreludeExt.CreateEitherAsync<ISetBlobTextErrors, Unit>(
                async () => {
                    try
                    {
                        var accessCondition = eTag! == null! ?
                            AccessCondition.GenerateIfNotExistsCondition() :
                            AccessCondition.GenerateIfMatchCondition(eTag.Value);

                        await blob.UploadTextAsync(text, null, accessCondition, null, null);

                        return Prelude.unit;
                    }
                    catch (StorageException e) when (e.RequestInformation.ErrorCode == BlobErrorCodeStrings.BlobAlreadyExists)
                    {
                        return new CouldNotCreateBlobBecauseItAlreadyExistsError(e, $"writing text to blob {blob.StorageUri}");
                    }
                    catch (StorageException e) when (e.RequestInformation.ErrorCode == StorageErrorCodeStrings.ConditionNotMet)
                    {
                        return new CouldNotUpdateBlobBecauseTheETagHasChanged(e, $"writing text to blob {blob.StorageUri}");
                    }
                    catch (Exception e)
                    {
                        return new GeneralStorageError(e, $"writing text to blob {blob.StorageUri}");
                    }
                });
        }

    }
}
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Blob.Protocol;
using Microsoft.Azure.Storage.Shared.Protocol;
using WebTests.Helpers;
using Xunit;

namespace WebTests
{
    public class BlobFunctionsTests
    {
        [Fact]
        public async Task Test()
        {
            var connectionString = TestConfiguration.GetStorageConnectionString();

            if (!CloudStorageAccount.TryParse(connectionString.Value, out var account))
                throw new InvalidOperationException("Connection string not valid");

            var serviceClient = account.CreateCloudBlobClient();
            var container = serviceClient.GetContainerReference("vaults");
            var cloudBlob = container.GetBlockBlobReference("steve@apple.com/index");

            var firstText = "First";
            var secondText = "Second";
            var thirdText = "Third";
            var fourthText = "Fourth";

            await cloudBlob.DeleteIfExistsAsync();

            try
            {
                await cloudBlob.DownloadTextAsync(Encoding.UTF8, null, null, null);

                Assert.False(true);
            }
            catch (StorageException e)
            {
                Assert.Equal(BlobErrorCodeStrings.BlobNotFound, e.RequestInformation.ErrorCode);
            }

            await cloudBlob.UploadTextAsync(firstText, Encoding.UTF8, AccessCondition.GenerateIfNotExistsCondition(), null, null);

            var etagFromUpload = cloudBlob.Properties.ETag;

            try
            {
                await cloudBlob.UploadTextAsync(secondText, Encoding.UTF8, AccessCondition.GenerateIfNotExistsCondition(), null, null);
            }
            catch (StorageException e)
            {
                Assert.Equal(BlobErrorCodeStrings.BlobAlreadyExists, e.RequestInformation.ErrorCode);
            }

            var text = await cloudBlob.DownloadTextAsync(Encoding.UTF8, null, null, null);

            Assert.Equal(firstText, text);

            var etagFromDownload = cloudBlob.Properties.ETag;

            Assert.Equal(etagFromDownload, etagFromUpload);

            await cloudBlob.UploadTextAsync(thirdText, Encoding.UTF8, AccessCondition.GenerateIfMatchCondition(etagFromUpload), null, null);

            try
            {
                await cloudBlob.UploadTextAsync(fourthText, Encoding.UTF8, AccessCondition.GenerateIfMatchCondition(etagFromUpload), null, null);

                Assert.False(true);
            }
            catch (StorageException e)
            {
                Assert.Equal(StorageErrorCodeStrings.ConditionNotMet, e.RequestInformation.ErrorCode);
            }

            var textAgain = await cloudBlob.DownloadTextAsync(Encoding.UTF8, null, null, null);

            Assert.Equal(thirdText, textAgain);

            Assert.True(await cloudBlob.ExistsAsync());
        }
    }
}

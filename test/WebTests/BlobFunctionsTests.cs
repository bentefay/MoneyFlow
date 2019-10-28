using System;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Xunit;
using ConfigurationFunctions = WebTests.Helpers.ConfigurationFunctions;

namespace WebTests
{
    public class BlobFunctionsTests
    {
        [Fact]
        public async Task Test()
        {
            var connectionString = ConfigurationFunctions.GetStorageConnectionString();
            
            if (!CloudStorageAccount.TryParse(connectionString.Value, out var account))
                throw new InvalidOperationException("Connection string not valid");

            var serviceClient = account.CreateCloudBlobClient();
            var container = serviceClient.GetContainerReference("vaults");
            var cloudBlob = container.GetBlockBlobReference("steve@apple.com/index");
            Assert.False(await cloudBlob.ExistsAsync());
        }
    }
}

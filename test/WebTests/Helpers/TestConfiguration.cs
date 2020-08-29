using Microsoft.Extensions.Configuration;
using Web.Functions;
using Web.Types.Values;

namespace WebTests.Helpers
{
    public static class TestConfiguration
    {
        public static StorageConnectionString GetStorageConnectionString() =>
            new ConfigurationBuilder()
                .AddUserSecrets<Web.AssemblyData>()
                .Build()
                .GetStorageConnectionString();
    }
}
using Microsoft.Extensions.Configuration;
using Web;
using Web.Functions;
using Web.Types;
using Web.Types.Values;

namespace WebTests.Helpers
{
    public static class TestConfiguration
    {
        public static StorageConnectionString GetStorageConnectionString() =>
            new ConfigurationBuilder()
                .AddUserSecrets<Startup>()
                .Build()
                .GetStorageConnectionString();
    }
}
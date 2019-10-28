using Microsoft.Extensions.Configuration;
using Web;
using Web.Functions;
using Web.Types;

namespace WebTests.Helpers
{
    public static class ConfigurationFunctions
    {
        public static StorageConnectionString GetStorageConnectionString() =>
            new ConfigurationBuilder()
                .AddUserSecrets<Startup>()
                .Build()
                .GetStorageConnectionString();
    }
}
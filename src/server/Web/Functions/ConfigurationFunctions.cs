using System;
using Microsoft.Extensions.Configuration;
using Web.Types;
using Web.Types.Values;

namespace Web.Functions
{
    public static class ConfigurationFunctions
    {
        public static StorageConnectionString GetStorageConnectionString(this IConfiguration configuration) =>
            new StorageConnectionString(configuration.GetRequiredValue<string>("StorageConnectionString"));

        public static T GetRequiredValue<T>(this IConfiguration configuration, string key) where T : class =>
            configuration.GetValue<T>(key) ?? throw new ConfigurationException($"Configuration is missing required key {key}");
    }

    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message) : base(message)
        {

        }
    }
}
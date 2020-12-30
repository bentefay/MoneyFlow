using System;
using Microsoft.Extensions.Configuration;
using Web.Types.Values;

namespace Web.Functions
{
    public static class ConfigurationFunctions
    {
        public static StorageConnectionString GetStorageConnectionString(this IConfiguration configuration) =>
            new StorageConnectionString(configuration.GetNonEmptyValue("StorageConnectionString"));

        public static T GetRequiredValue<T>(this IConfiguration configuration, string key) where T : class =>
            configuration.GetValue<T>(key) ?? throw new ConfigurationException($"Configuration is missing required key {key}");

        public static string GetNonEmptyValue(this IConfiguration configuration, string key)
        {
            var value = configuration.GetRequiredValue<string>(key);

            if (string.IsNullOrWhiteSpace(value))
                throw new ConfigurationException($"Configuration key {key} has empty or whitespace value");

            return value;
        }
    }

    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message) : base(message)
        {

        }
    }
}
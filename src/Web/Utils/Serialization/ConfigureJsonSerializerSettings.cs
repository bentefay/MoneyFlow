using Newtonsoft.Json;
using Serilog;

namespace Web.Utils.Serialization
{
    public static class ConfigureJsonSerializerSettings
    {
        public static JsonSerializerSettings Execute(JsonSerializerSettings settings)
        {
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.DefaultValueHandling = DefaultValueHandling.Include;
            settings.TypeNameHandling = TypeNameHandling.None;
            settings.Converters.Add(new OptionJsonConverter());
            settings.ContractResolver = new OptionContractResolver(settings.NullValueHandling);
            settings.Error += (sender, args) => 
                Log.Warning(
                    args.ErrorContext.Error, 
                    $"JSON deserialization error on type '{args.CurrentObject.GetType().Name}' at path '{args.ErrorContext.Path}'");
            return settings;
        }
    }
}

using Newtonsoft.Json;
using Web.Utils.Serialization.Converters.SumTypes;

namespace Web.Types
{
    [SumType(nameof(Type))]
    [JsonConverter(typeof(SingleAssemblySumTypeConverter<ISumType>))]
    public interface ISumType
    {
        string Type { get; }   
    }
}
using LanguageExt;
using Newtonsoft.Json;
using Web.Types.Errors;

namespace Web.Functions
{
    public static class JsonFunctions
    {
        public static Either<JsonDeserializationError, T> Deserialize<T>(string json, JsonSerializerSettings settings)
        {
            try
            {               
                return JsonConvert.DeserializeObject<T>(json, settings);
            }
            catch (JsonReaderException e)
            {
                return new JsonDeserializationError(typeof(T), json, e.Path, e.LineNumber, e.LinePosition, e.Message);
            }
        }
        
        public static Either<JsonSerializationError, Unit> Serialize<T>(T value, JsonSerializerSettings settings)
        {
            try
            {               
                JsonConvert.SerializeObject(value, settings);
                return Prelude.unit;
            }
            catch (JsonWriterException e)
            {
                return new JsonSerializationError(typeof(T), value, e.Path, e.Message);
            }
        }
    }
}
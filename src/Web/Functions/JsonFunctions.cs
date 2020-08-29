using System.Reflection.Metadata;
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
            catch (JsonSerializationException e)
            {
                return new JsonDeserializationError(typeof(T), json, e.Path!, e.LineNumber, e.LinePosition, e);
            }
            catch (JsonReaderException e)
            {
                return new JsonDeserializationError(typeof(T), json, e.Path!, e.LineNumber, e.LinePosition, e);
            }
        }
        
        public static Either<JsonSerializationError, string> Serialize<T>(T value, JsonSerializerSettings settings)
        {
            try
            {               
                return JsonConvert.SerializeObject(value, settings);
            }
            catch (JsonSerializationException e)
            {
                return new JsonSerializationError(typeof(T), value, e.Path!, e);
            }
            catch (JsonWriterException e)
            {
                return new JsonSerializationError(typeof(T), value, e.Path!, e);
            }
        }
    }
}
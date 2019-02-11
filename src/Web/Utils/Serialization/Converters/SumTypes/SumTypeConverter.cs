using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Web.Utils.Extensions;

namespace Web.Utils.Serialization.Converters.SumTypes
{
    public class SumTypeConverter : JsonConverter
    {
        public static SumTypeConverterBuilder<T> For<T>() => SumTypeConverterBuilder<T>.Create();

        private readonly Type _baseType;
        private readonly string _discriminatorPropertyName;
        private readonly IReadOnlyDictionary<string, Type> _discriminatorToSubTypeMap;

        [ThreadStatic] private static bool _isInsideRead;
        [ThreadStatic] private static JsonReader _reader;

        public override bool CanRead => !(_isInsideRead && string.IsNullOrEmpty(_reader.Path));

        public override bool CanWrite => false;

        public SumTypeConverter(Type baseType, string discriminatorPropertyName, IReadOnlyDictionary<string, Type> discriminatorToSubTypeMap)
        {
            _baseType = baseType;
            _discriminatorPropertyName = discriminatorPropertyName;
            _discriminatorToSubTypeMap = discriminatorToSubTypeMap.ToDictionary(x => x.Key, x => x.Value);
        }

        public override bool CanConvert(Type objectType) =>
            objectType.ImplementsOrExtendsWithOpenTypes(_baseType);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            throw new NotImplementedException();

        public override object ReadJson(JsonReader reader, Type sumType, object _, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null || reader.TokenType == JsonToken.None)
                return null;

            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonException(
                    $"Could not deserialize JSON into type '{_baseType.Name}' because the JSON should be an object but is actually a '{reader.TokenType}'");

            var jObject = JObject.Load(reader);

            var subType = CreateSubTypeWithSameGenericParametersAsSumType(
                GetSubTypeFromDiscriminator(jObject),
                sumType);

            _reader = CloneReaderFor(jObject, reader);

            _isInsideRead = true;
            try
            {
                return serializer.Deserialize(_reader, subType);
            }
            finally
            {
                _isInsideRead = false;
            }
        }

        private Type CreateSubTypeWithSameGenericParametersAsSumType(Type subType, Type sumType)
        {
            return subType.IsGenericType && sumType.IsGenericType
                ? subType
                    .GetGenericTypeDefinition()
                    .MakeGenericType(sumType.GetGenericArguments())
                : subType;
        }

        private Type GetSubTypeFromDiscriminator(JObject jObject)
        {
            var discriminatorToken = jObject.GetValue(_discriminatorPropertyName, StringComparison.OrdinalIgnoreCase);

            if (discriminatorToken == null)
                throw new JsonException(
                    $"Could not deserialize JSON into type '{_baseType.Name}' because the JSON object is missing the discriminator property " +
                    $"'{_discriminatorPropertyName}', which is required.\n\n" +
                    Context());

            if (discriminatorToken.Type != JTokenType.String)
                throw new JsonException(
                    $"Could not deserialize JSON into type '{_baseType.Name}' because the JSON object contains the discriminator property '{_discriminatorPropertyName}' " +
                    $"which must be of JSON type '{nameof(JTokenType.String)}' but is of JSON type '{discriminatorToken.Type}' ({discriminatorToken}).\n\n" +
                    Context());

            var discriminator = discriminatorToken.Value<string>();

            if (!_discriminatorToSubTypeMap.TryGetValue(discriminator, out var targetType))
                throw new JsonException(
                    $"Could not deserialize JSON into type '{_baseType.Name}' because the JSON object contains the discriminator property '{_discriminatorPropertyName}' " +
                    $"with an unknown value of '{discriminator}'. Make sure that the discriminator '{discriminator}' has been mapped in the {typeof(SumTypeConverter).Name} " +
                    $"for type '{_baseType.Name}'.\n\n" +
                    Context());

            return targetType;
        }

        private string Context() =>
            $"This discriminator is required in order for the {typeof(SumTypeConverter).Name} to know which subtype of {_baseType} to deserialize into.";

        private static JsonReader CloneReaderFor(JToken jToken, JsonReader reader)
        {
            var jObjectReader = jToken.CreateReader();
            jObjectReader.Culture = reader.Culture;
            jObjectReader.CloseInput = reader.CloseInput;
            jObjectReader.SupportMultipleContent = reader.SupportMultipleContent;
            jObjectReader.DateTimeZoneHandling = reader.DateTimeZoneHandling;
            jObjectReader.FloatParseHandling = reader.FloatParseHandling;
            jObjectReader.DateFormatString = reader.DateFormatString;
            jObjectReader.DateParseHandling = reader.DateParseHandling;
            return jObjectReader;
        }
    }
}

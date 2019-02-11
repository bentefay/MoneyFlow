using System;
using System.Reflection;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using Newtonsoft.Json;

namespace Web.Utils.Serialization.Converters
{
    public class OptionJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType
                && objectType.GetGenericTypeDefinition() == typeof(Option<>);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var wrappedType = value.GetType().GetGenericArguments()[0];

            typeof(OptionJsonConverter)
                .GetMethod(nameof(WriteJson), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(wrappedType)
                .Invoke(null, new[] { writer, serializer, value });
        }

        private static void WriteJson<T>(JsonWriter writer, JsonSerializer serializer, Option<T> option)
        {
            if (option.IsSome)
                serializer.Serialize(writer, option.ValueUnsafe());
            else
                writer.WriteNull();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var wrappedType = objectType.GetGenericArguments()[0];

            return typeof(OptionJsonConverter)
                .GetMethod(
                    wrappedType.IsValueType ?
                        nameof(ReadJsonValueType) :
                        nameof(ReadJson),
                    BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(wrappedType)
                .Invoke(null, new object[] { reader, serializer });
        }

        private static Option<T> ReadJsonValueType<T>(JsonReader reader, JsonSerializer serializer) where T : struct =>
            Prelude.Optional(serializer.Deserialize<T?>(reader));

        private static Option<T> ReadJson<T>(JsonReader reader, JsonSerializer serializer) =>
            Prelude.Optional(serializer.Deserialize<T>(reader));
    }
}

using System;
using Newtonsoft.Json;

// ReSharper disable StaticMemberInGenericType

namespace Web.Utils.Serialization.Converters.SumTypes
{
    /// <summary>
    /// Use this converter in a [JsonConverter] attribute to automatically discover types in this assembly
    /// </summary>
    public class SingleAssemblySumTypeConverter<T> : JsonConverter
    {
        private static volatile SumTypeConverter _converter;
        private static readonly object _lock = new object();

        private SumTypeConverter GetConverter()
        {
            if (_converter == null)
            {
                lock (_lock)
                {
                    if (_converter == null)
                    {
                        _converter = SumTypeConverter.For<T>()
                            .AddSubtypesInContainingAssembly()
                            .Build();
                    }
                }
            }

            return _converter;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            GetConverter().WriteJson(writer, value, serializer);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
            GetConverter().ReadJson(reader, objectType, existingValue, serializer);

        public override bool CanConvert(Type objectType) =>
            GetConverter().CanConvert(objectType);

        public override bool CanRead =>
            GetConverter().CanRead;

        public override bool CanWrite =>
            GetConverter().CanWrite;
    }
}

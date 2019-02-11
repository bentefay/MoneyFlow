using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using LanguageExt;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Web.Types;

namespace Web.Utils.Serialization.Converters
{
    public class TinyTypeJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) =>
            ToHandledType(objectType).IsSome;

        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer
        ) =>
            ToHandledType(value.GetType())
                .Map(i => i.GetProperty(nameof(ITinyType<object>.Value)))
                .Iter(prop => serializer.Serialize(writer, prop.GetValue(value)));

        public override object ReadJson(
            JsonReader reader,
            Type incomingType,
            object existingValue,
            JsonSerializer serializer
        )
        {
            var jToken = JToken.Load(reader);
            return
            (
                from targetType in ToHandledType(incomingType).ToEither("bad thing")
                from typeOfValueProp in GetValuePropertyType(targetType)
                from convertedValue in ConvertToValueOfType(jToken, typeOfValueProp)
                from constructor in ResolveConstructor(targetType, typeOfValueProp)
                select constructor.Invoke(new[] { convertedValue })
            ).Match(
                Left: errorMsg => throw new SerializationException(
                    $"Could not deserialize json input value `{jToken}` to a `{incomingType.Name}` " +
                    $"tiny type. {errorMsg}"
                ),
                Right: Prelude.identity
            );
        }

        private static Option<Type> ToHandledType(Type objectType)
        {
            bool IsNullable() => objectType.IsGenericType
                && objectType.GetGenericTypeDefinition() == typeof(Nullable<>);

            return Prelude.Optional(
                IsNullable()
                ? objectType.GetGenericArguments()[0]
                : objectType
            ).Bind(t => ImplementsTinyTypeInterface(t) ? Prelude.Some(t) : Prelude.None);
        }

        public static bool ImplementsTinyTypeInterface(Type objectType) =>
            Prelude.Optional(
                objectType
                    ?.GetInterfaces()
                    .Where(i => i.IsGenericType)
                    .FirstOrDefault(i => i.GetGenericTypeDefinition() == typeof(ITinyType<>))
            ).IsSome;

        public static Either<string, Type> GetValuePropertyType(Type type) =>
            Prelude.Optional(
                type?.GetProperty(nameof(ITinyType<object>.Value))?.PropertyType
            )
            .ToEither(
                $"There was no `{nameof(ITinyType<object>.Value)}` property."
            );

        private static Either<string, ConstructorInfo> ResolveConstructor(Type tinyType, Type parameterType) =>
            Prelude.Optional(
                tinyType.GetConstructor(new[] { parameterType })
            )
            .Match<Either<string, ConstructorInfo>>(
                None: () => Prelude.Left(
                    $"There is no unary constructor which takes an arg of type `{parameterType.Name}`."
                ),
                Some: ci => Prelude.Right(ci)
            );

        private static Either<string, object> ConvertToValueOfType(JToken jToken, Type targetType)
        {
            try
            {
                return Prelude.Right(jToken.ToObject(targetType));
            }
            catch (Exception e)
            {
                return Prelude.Left(
                    $"The input value could not be converted to a value of type `{targetType.Name}`. " +
                    $"Exception message was: '{e.Message}'"
                );
            }
        }

    }
}

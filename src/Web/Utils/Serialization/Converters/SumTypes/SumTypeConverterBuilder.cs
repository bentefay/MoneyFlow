using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Web.Types;
using Web.Utils.Extensions;

namespace Web.Utils.Serialization.Converters.SumTypes
{
    public class SumTypeConverterBuilder<T>
    {
        private readonly string _discriminatorPropertyName;
        private readonly Dictionary<string, Type> _discriminatorToSubTypeMap = new Dictionary<string, Type>();

        public static SumTypeConverterBuilder<T> Create()
        {
            var sumTypeAttributes = typeof(T).GetCustomAttributesInherited<SumTypeAttribute>().ToList();

            if (sumTypeAttributes.Count == 0)
                throw new InvalidOperationException(
                    $"Could not create a {nameof(SumTypeConverter)} for '{typeof(T).Name}' because it does not have a {nameof(SumTypeAttribute)}. " +
                    $"Please add a {nameof(SumTypeAttribute)} to '{typeof(T)}'. This attribute is required to allow for convention tests to easily " +
                    $"enforce invariants about your SumTypes.");

            return new SumTypeConverterBuilder<T>(sumTypeAttributes.First().DiscriminatorPropertyName);
        }

        public SumTypeConverterBuilder(string discriminatorPropertyName)
        {
            _discriminatorPropertyName = discriminatorPropertyName;
        }

        public SumTypeConverterBuilder<T> AddSubtypesInContainingAssembly() =>
            AddSubtypesInAssembly(typeof(T).Assembly);

        public SumTypeConverterBuilder<T> AddSubtypesInAssembly(params Assembly[] assemblies)
        {
            var sumType = typeof(T);

            var subTypes = assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(subtype => subtype.ImplementsOrExtendsWithOpenTypes(sumType) && sumType != subtype && !subtype.IsInterface && !subtype.IsAbstract)
                .Select(subtype => (discriminator: GetDiscriminator(subtype), Type: subtype))
                .ToList();

            foreach (var subType in subTypes)
            {
                AddSubtype(subType.discriminator, subType.Type);
            }

            return this;
        }

        public SumTypeConverterBuilder<T> AddSubtype<TSubtype>() where TSubtype : T =>
            AddSubtype(GetDiscriminator(typeof(TSubtype)), typeof(TSubtype));

        private SumTypeConverterBuilder<T> AddSubtype(string discriminator, Type type)
        {
            if (_discriminatorToSubTypeMap.TryGetValue(discriminator, out var existingType) && existingType != type)
                throw new InvalidOperationException(
                    $"Could not add a new mapping from discriminator '{discriminator}' to type '{type.Name}' for sum type '{typeof(T).Name}', " +
                    $"because this discriminator is already mapped to type '{_discriminatorToSubTypeMap[discriminator].Name}'. " +
                    $"This discriminator must be unique because {nameof(SumTypeConverter)} uses this " +
                    $"property to know which subtype of '{typeof(T).Name}' to deserialize into.");

            _discriminatorToSubTypeMap[discriminator] = type;

            return this;
        }

        public SumTypeConverter Build()
        {
            if (!_discriminatorToSubTypeMap.Any())
                throw new InvalidOperationException($"Trying to build a {nameof(SumTypeConverter)} for '{typeof(T).Name}' but no subtypes have been registered");

            return new SumTypeConverter(typeof(T), _discriminatorPropertyName, _discriminatorToSubTypeMap);
        }

        private string GetDiscriminator(Type type)
        {
            var concreteType = type.CreateExampleConcreteTypeFromOpenGeneric();

            var discriminatorProperty = concreteType.GetProperty(_discriminatorPropertyName);

            if (discriminatorProperty == null)
                throw new InvalidOperationException(
                    $"'{type.Name}' does not contain a property '{_discriminatorPropertyName}', but this property was expected. " +
                    $"A property with the name '{_discriminatorPropertyName}' must be present because {nameof(SumTypeConverter)} uses this " +
                    $"property to know which subtype of '{typeof(T).Name}' to deserialize into.");

            if (discriminatorProperty.PropertyType != typeof(string))
                throw new InvalidOperationException(
                    $"'{type.Name}' has a property '{_discriminatorPropertyName}' which has a type of {discriminatorProperty.PropertyType}, but a type of string was expected. " +
                    $"The value of this property '{_discriminatorPropertyName}' must be a string because {nameof(SumTypeConverter)} uses this " +
                    $"property to know which subtype of '{typeof(T).Name}' to deserialize into.");

            var instance = FormatterServices.GetUninitializedObject(concreteType);

            var discriminator = discriminatorProperty.GetValue(instance);

            if (discriminator == null)
                throw new InvalidOperationException(
                    $"Instance of '{type.Name}' has a property '{_discriminatorPropertyName}' which is null, but a non-null value was expected. " +
                    $"The value of this discriminator property '{_discriminatorPropertyName}' must be non-null because {nameof(SumTypeConverter)} uses this " +
                    $"property to know which subtype of '{typeof(T).Name}' to deserialize into. It is possible this property is null because you " +
                    $"are setting it in a constructor (or field initializer). Instead, make sure this property is a lambda that immediately returns " +
                    $"a string literal or const.");

            return (string)discriminator;
        }
    }
}

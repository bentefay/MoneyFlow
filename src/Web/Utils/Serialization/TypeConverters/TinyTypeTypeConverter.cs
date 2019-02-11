using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using LanguageExt;
using Web.Types;
using Web.Utils.Extensions;

namespace Web.Utils.Serialization.TypeConverters
{
    public class TinyTypeTypeConverter<T, TInner> : TypeConverter where T : ITinyType<TInner>
    {
        private readonly Func<object, object> _tinyTypeCreator;

        public TinyTypeTypeConverter()
        {
            var tinyTypeType = typeof(T);
            var constructor = tinyTypeType.GetConstructor(new [] {typeof(TInner)});
            if (constructor == null)
                throw new ArgumentException($"TinyType {tinyTypeType} is missing primitive constructor", nameof(tinyTypeType));
            _tinyTypeCreator = v => constructor.Invoke(new[] {v});
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType.IsPrimitive || sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var innerValue = value is TInner ? value : TypeDescriptor.GetConverter(typeof(TInner)).ConvertFrom(value);
            return _tinyTypeCreator(innerValue);
        }
    }
    
    public static class TinyTypeTypeConverter
    {
        public static IDisposable ScanForAndRegisterTinyTypeTypeConverters(Assembly assembly) =>
            ScanForAndRegisterTinyTypeTypeConverters(new[] {assembly});
        
        public static IDisposable ScanForAndRegisterTinyTypeTypeConverters(params Assembly[] assemblies) =>
            ScanForAndRegisterTinyTypeTypeConverters(assemblies.SelectMany(x => x.GetTypes()));

        public static IDisposable ScanForAndRegisterTinyTypeTypeConverters(params Type[] allTypes) =>
            ScanForAndRegisterTinyTypeTypeConverters(allTypes.AsEnumerable());
        
        public static IDisposable ScanForAndRegisterTinyTypeTypeConverters(IEnumerable<Type> allTypes) =>
            allTypes
                .Where(ImplementsTinyType)
                .Apply(RegisterTinyTypeTypeConverters);

        private static bool ImplementsTinyType(Type type) =>
            type
                .GetInterfaces()
                .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ITinyType<>));

        public static IDisposable RegisterTinyTypeTypeConverters(IEnumerable<Type> tinyTypes) =>
            tinyTypes
                .Select(tinyType =>
                {
                    var innerT = GetTinyTypeInnerType(tinyType);
                    return (tinyType,
                        TypeDescriptor.AddAttributes(tinyType,
                            new TypeConverterAttribute(
                                typeof(TinyTypeTypeConverter<,>).MakeGenericType(tinyType, innerT)))
                        );
                })
                .ToImmutableList()
                .Apply(list => Disposable.Create(() => list.Iter(tuple =>
                {
                    var (tinyType, provider) = tuple;
                    TypeDescriptor.RemoveProvider(provider, tinyType);
                })));
        
        private static Type GetTinyTypeInnerType(Type tinyTypeType)
        {
            var typeSet = tinyTypeType
                .GetInterfaces()
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ITinyType<>))
                .ToReadOnlyList();

            if (typeSet.Count != 1)
                throw new ArgumentException($"{tinyTypeType} arg must implement a single ITinyType<> interface",
                    nameof(tinyTypeType));

            var tinyTypeInterface = typeSet.Single();
            return tinyTypeInterface.GetGenericArguments().Single();
        }
    }
}

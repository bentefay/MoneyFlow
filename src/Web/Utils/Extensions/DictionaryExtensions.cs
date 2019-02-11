using System;
using System.Collections.Generic;
using LanguageExt;

namespace Web.Utils.Extensions
{
    public static class DictionaryExtensions
    {
        public static Option<TValue> GetValue<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key) =>
            @this.TryGetValue(key, out var value) ? Prelude.Some(value) : Prelude.None;
        
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, Func<TValue> factory)
        {
            if (@this.TryGetValue(key, out var value))
            {
                return value;
            }
            else
            {
                var defaultValue = factory();
                @this[key] = defaultValue;
                return defaultValue;
            }
        }
    }
}

using System.Collections.Generic;
using LanguageExt;

namespace Make.Utility.Extensions
{
    public static class StringExtensions 
    {
        public static string Join(this IEnumerable<object> source, string separator)
        {
            return string.Join(separator, source);
        }
        
        public static Option<int> ParseInt(this string s)
        {
            return int.TryParse(s, out var i) ? Prelude.Some(i) : Prelude.None;
        }
    }
}
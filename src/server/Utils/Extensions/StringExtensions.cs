using System;
using System.Linq;

namespace Web.Utils.Extensions
{
    public static class StringExtensions
    {
        public static string WithMaxLength(this string @this, int maxLength, string terminator = "...")
        {
            return @this.Length > maxLength ?
                @this.Substring(0, maxLength) + terminator :
                @this;
        }

        public static string ToCamelCase(this string @this)
        {
            return string.Concat(@this.Select((x, i) => i == 0 ? char.ToLower(x) : x));
        }

        public static string Indent(this string @this, string indent = "\t")
        {
            return @this
                .Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                .Select(line => string.IsNullOrWhiteSpace(line) ? line : indent + line)
                .Join("\n");
        }

        public static string TrimEnd(this string @this, string end)
        {
            return @this.EndsWith(end) ? @this[..^end.Length] : @this;
        }
    }
}

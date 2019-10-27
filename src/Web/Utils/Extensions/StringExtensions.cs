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

        public static string Indent(this string @this, string indent = "\t")
        {
            return @this
                .Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                .Select(line => string.IsNullOrWhiteSpace(line) ? line : indent + line)
                .Join("\n");
        }
    }
}

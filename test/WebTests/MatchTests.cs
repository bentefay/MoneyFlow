using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Web.Utils.Extensions;
using Xunit;
using Xunit.Sdk;

namespace WebTests
{
    public class MatchTests
    {
        [Fact]
        public void AllMatchStatementsMatchAllVariants()
        {
            var allTypes = Assemblies.All.SelectMany(x => x.Types.Value).ToList();

            var allFunctions = allTypes
                .SelectMany(x => x.GetMethods(BindingFlags.Public | BindingFlags.Static))
                .Filter(x => x.Name.StartsWith("Match"))
                .ToReadOnlyList();

            foreach (var function in allFunctions)
            {
                var sumType = function.GetParameters()[0].ParameterType;

                var expectedVariantsMatched = allTypes
                    .Filter(x => x.ImplementsOrExtends(sumType) && x.IsClass && !x.IsAbstract)
                    .Distinct()
                    .ToImmutableHashSet();

                var actualVariantsMatched = function
                    .GetParameters()
                    .Skip(1)
                    .Select(x => x.ParameterType.GenericTypeArguments[0])
                    .Distinct()
                    .ToImmutableHashSet();

                if (expectedVariantsMatched.SetEquals(actualVariantsMatched)) continue;
                
                var unneeded = actualVariantsMatched.Except(expectedVariantsMatched);
                var missing = expectedVariantsMatched.Except(actualVariantsMatched);
                var parameters = expectedVariantsMatched.Select(x => $"Func<{x.Name}, T> {FormatVariantName(x)}").Join(", ");
                var cases = expectedVariantsMatched.Select(x => $"{x.Name} e => {FormatVariantName(x)}(e),").Join("\n");
                var implementation = $"public static T Match<T>(this {sumType.Name} @this, {parameters})\n" +
                                     $"{{\n" +
                                     $"return @this switch {{\n" +
                                     $"{cases}\n" +
                                     $"_ => throw new NotImplementedException(\"Missing match for \" + @this.GetType().Name)\n" +
                                     $"}};\n" +
                                     $"}}\n\n";
                
                throw new XunitException($"Match function '{function.DeclaringType.FullName}.{function.Name}' is incorrect.\n" +
                                         $"Missing types:  {missing.Select(x => x.Name).Join(", ")}\n" +
                                         $"Unneeded types: {unneeded.Select(x => x.Name).Join(", ")}\n" +
                                         $"Expected implementation:\n\n" +
                                         $"{implementation}"
                );
            }

            string FormatVariantName(Type variant)
            {
                return variant.Name.ToCamelCase().TrimEnd("Error");
            }
        }
    }
}
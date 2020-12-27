using System.Text.RegularExpressions;
using LanguageExt;
using Web.Types.Errors;

namespace Web.Types.Values
{
    public record Email(string Value) : ITinyType<string>
    {
        private static readonly Regex EmailRegex = new(@"^[^\s@]+@[^\s@]+\.[^\s@]{2,}$");
        
        public static Either<MalformedEmail, Email> Create(string value) => 
            !string.IsNullOrWhiteSpace(value) && EmailRegex.IsMatch(value) ?
                Prelude.Right(new Email(value)) :
                Prelude.Left<MalformedEmail, Email>(new MalformedEmail(value));
    }
}
using System.Text.RegularExpressions;
using LanguageExt;
using Web.Types.Errors;

namespace Web.Types.Values
{
    public class Email : TinyType<Email, string>
    {
        private static readonly Regex _email = new Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]{2,}$");
        
        public static Either<MalformedEmail, Email> Create(string value) => 
            !string.IsNullOrWhiteSpace(value) && _email.IsMatch(value) ?
                Prelude.Right(new Email(value)) :
                Prelude.Left<MalformedEmail, Email>(new MalformedEmail(value));

        private Email(string value) : base(value)
        {
        }
    }
}
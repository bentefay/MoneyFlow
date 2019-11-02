using LanguageExt;
using Web.Types.Errors;

namespace Web.Types.Values
{
    public class DoubleHashedPassword : TinyType<DoubleHashedPassword, string>
    {
        public static Either<MalformedPassword, DoubleHashedPassword> Create(string value) => 
            !string.IsNullOrWhiteSpace(value) ?
                Prelude.Right<DoubleHashedPassword>(new DoubleHashedPassword(value)) :
                Prelude.Left<MalformedPassword, DoubleHashedPassword>(new MalformedPassword(value));

        public DoubleHashedPassword(string value) : base(value)
        {
        }
    }
}
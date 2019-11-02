using LanguageExt;
using Web.Types.Errors;

namespace Web.Types.Values
{
    public class DoubleHashedPassword : TinyType<DoubleHashedPassword, string>
    {
        public static Either<InvalidHashedPassword, DoubleHashedPassword> Create(string value) => 
            !string.IsNullOrWhiteSpace(value) ?
                Prelude.Right<DoubleHashedPassword>(new DoubleHashedPassword(value)) :
                Prelude.Left<InvalidHashedPassword, DoubleHashedPassword>(new InvalidHashedPassword(value));

        public DoubleHashedPassword(string value) : base(value)
        {
        }
    }
}
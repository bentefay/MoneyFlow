using LanguageExt;
using Web.Types.Errors;

namespace Web.Types.Values
{
    public class HashedPassword : TinyType<HashedPassword, string>
    {
        public static Either<MalformedPassword, HashedPassword> Create(string value) => 
            !string.IsNullOrWhiteSpace(value) ?
                Prelude.Right(new HashedPassword(value)) :
                Prelude.Left<MalformedPassword, HashedPassword>(new MalformedPassword(value));

        public HashedPassword(string value) : base(value)
        {
        }
    }
}
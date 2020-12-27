using LanguageExt;
using Web.Types.Errors;

namespace Web.Types.Values
{
    public record HashedPassword(string Value)
    {
        public static Either<MalformedPassword, HashedPassword> Create(string value) => 
            !string.IsNullOrWhiteSpace(value) ?
                Prelude.Right(new HashedPassword(value)) :
                Prelude.Left<MalformedPassword, HashedPassword>(new MalformedPassword(value));
    }
}
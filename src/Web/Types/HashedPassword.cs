using LanguageExt;
using Web.Types.Errors;

namespace Web.Types
{
    public struct HashedPassword : ITinyType<string>
    {
        public static Either<InvalidHashedPassword, HashedPassword> Create(string value) => 
            !string.IsNullOrWhiteSpace(value) ?
                Prelude.Right(new HashedPassword(value)) :
                Prelude.Left<InvalidHashedPassword, HashedPassword>(new InvalidHashedPassword(value));
        
        private HashedPassword(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}
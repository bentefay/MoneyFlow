using LanguageExt;
using Web.Types.Errors;

namespace Web.Types
{
    public class StorageETag : TinyType<StorageETag, string>
    {
        public static Either<InvalidHashedPassword, StorageETag> Create(string value) => 
            !string.IsNullOrWhiteSpace(value) ?
                Prelude.Right(new StorageETag(value)) :
                Prelude.Left<InvalidHashedPassword, StorageETag>(new InvalidHashedPassword(value));

        private StorageETag(string value) : base(value)
        {
        }
    }
}
using LanguageExt;
using Web.Types.Errors;

namespace Web.Types.Values
{
    public class StorageETag : TinyType<StorageETag, string>
    {
        public static Either<InvalidStorageETag, StorageETag> Create(string value) => 
            !string.IsNullOrWhiteSpace(value) ?
                Prelude.Right(new StorageETag(value)) :
                Prelude.Left<InvalidStorageETag, StorageETag>(new InvalidStorageETag(value));

        private StorageETag(string value) : base(value)
        {
        }
    }
}
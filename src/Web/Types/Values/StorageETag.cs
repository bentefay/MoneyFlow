using LanguageExt;
using Web.Types.Errors;

namespace Web.Types.Values
{
    public class StorageETag : TinyType<StorageETag, string>
    {
        public static Either<MalformedETag, StorageETag> Create(string value) => 
            !string.IsNullOrWhiteSpace(value) ?
                Prelude.Right(new StorageETag(value)) :
                Prelude.Left<MalformedETag, StorageETag>(new MalformedETag(value));

        private StorageETag(string value) : base(value)
        {
        }
    }
}
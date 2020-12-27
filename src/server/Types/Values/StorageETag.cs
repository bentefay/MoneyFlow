using LanguageExt;
using Web.Types.Errors;

namespace Web.Types.Values
{
    public record StorageETag(string Value): ITinyType<string>
    {
        public static Either<MalformedETag, StorageETag> Create(string value) =>
            !string.IsNullOrWhiteSpace(value) ?
                Prelude.Right(new StorageETag(value)) :
                Prelude.Left<MalformedETag, StorageETag>(new MalformedETag(value));
}
}
using LanguageExt;
using Web.Types.Values;

namespace Web.Types
{
    public class StorageText : Record<StorageText>
    {
        public string Text { get; }
        public StorageETag ETag { get; }

        public StorageText(string text, StorageETag eTag)
        {
            Text = text;
            ETag = eTag;
        }
    }
}
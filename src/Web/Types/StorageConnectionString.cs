namespace Web.Types
{
    public class StorageConnectionString : TinyType<StorageConnectionString, string>
    {
        public StorageConnectionString(string value) : base(value)
        {
        }
    }
}
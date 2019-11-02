namespace Web.Types.Values
{
    public class StorageConnectionString : TinyType<StorageConnectionString, string>
    {
        public StorageConnectionString(string value) : base(value)
        {
        }
    }
}
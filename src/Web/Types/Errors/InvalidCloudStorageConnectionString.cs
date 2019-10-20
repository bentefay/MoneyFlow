namespace Web.Types.Errors
{
    internal class InvalidCloudStorageConnectionString : IError
    {
        public string GetDescription() => "Invalid storage connection string";
    }
}
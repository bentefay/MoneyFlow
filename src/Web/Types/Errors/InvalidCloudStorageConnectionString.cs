using Web.Functions;

namespace Web.Types.Errors
{
    internal class InvalidCloudStorageConnectionString : IError, IGetBlobErrors
    {
        public string GetDescription() => "Invalid storage connection string";
    }
}
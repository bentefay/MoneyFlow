using Web.Functions;

namespace Web.Types.Errors
{
    internal class MalformedCloudStorageConnectionString : IError, IGetBlobErrors
    {
        public string GetDescription() => "Storage connection string is badly formatted";
    }
}
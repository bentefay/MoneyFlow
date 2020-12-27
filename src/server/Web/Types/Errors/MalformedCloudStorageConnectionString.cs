using Web.Functions;

namespace Web.Types.Errors
{
    public class MalformedCloudStorageConnectionString : IError, IGetBlobErrors
    {
        public string GetDescription() => "Storage connection string is badly formatted";
    }
}
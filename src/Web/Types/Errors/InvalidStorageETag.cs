namespace Web.Types.Errors
{
    public class InvalidStorageETag : IError, IGetBlobTextErrors, IDeserializeVaultIndexErrors, IParseAuthorizationErrors, IUpdateVaultRequestToVaultErrors
    {
        private readonly string _etag;

        public InvalidStorageETag(string etag)
        {
            _etag = etag;
        }

        public string GetDescription() => $"ETag '{_etag}' is invalid";
    }
}
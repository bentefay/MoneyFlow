namespace Web.Types.Errors
{
    public class MalformedETag : IError, IGetBlobTextErrors, IDeserializeUserErrors, IParseAuthorizationErrors, IUpdateVaultRequestToVaultErrors
    {
        private readonly string _etag;

        public MalformedETag(string etag)
        {
            _etag = etag;
        }

        public string GetDescription() => $"ETag '{_etag}' is badly formatted";
    }
}
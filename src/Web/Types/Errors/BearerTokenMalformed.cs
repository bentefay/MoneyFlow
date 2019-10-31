namespace Web.Types.Errors
{
    public class BearerTokenMalformed : IError, IParseAuthorizationErrors
    {
        private readonly string _authorizationHeader;

        public BearerTokenMalformed(string authorizationHeader)
        {
            _authorizationHeader = authorizationHeader;
        }
        
        public string GetDescription() => $"Expected header Authorization: Bearer <Authorization> but got 'Authorization: {_authorizationHeader}'";
    }
}
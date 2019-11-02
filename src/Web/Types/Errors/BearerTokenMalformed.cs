namespace Web.Types.Errors
{
    public class BearerTokenMalformed : IError, IParseAuthorizationErrors
    {
        public string AuthorizationHeader { get; }

        public BearerTokenMalformed(string authorizationHeader)
        {
            AuthorizationHeader = authorizationHeader;
        }
        
        public string GetDescription() => $"Expected header Authorization: Bearer <Authorization> but got 'Authorization: {AuthorizationHeader}'";
    }
}
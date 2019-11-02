namespace Web.Types.Errors
{
    public class MalformedBearerToken : IError, IParseAuthorizationErrors
    {
        public string AuthorizationHeader { get; }

        public MalformedBearerToken(string authorizationHeader)
        {
            AuthorizationHeader = authorizationHeader;
        }
        
        public string GetDescription() => $"Expected header 'Authorization: Bearer <Authorization>' but got 'Authorization: {AuthorizationHeader}'";
    }
}
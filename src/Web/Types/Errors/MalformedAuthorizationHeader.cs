namespace Web.Types.Errors
{
    public class MalformedAuthorizationHeader : IError
    {
        private readonly string _authorizationHeader;

        public MalformedAuthorizationHeader(string authorizationHeader)
        {
            _authorizationHeader = authorizationHeader;
        }
        
        public string GetDescription() => $"Expected header Authorization: Bearer <Authorization> but got 'Authorization: {_authorizationHeader}'";
    }
}
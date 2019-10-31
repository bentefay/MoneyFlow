namespace Web.Types.Errors
{
    public class BearerTokenMissing : IError, IParseAuthorizationErrors
    {
        public string GetDescription() => "Expected header Authorization: Bearer <Authorization>";
    }
}
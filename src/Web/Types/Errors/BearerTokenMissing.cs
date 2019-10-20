namespace Web.Types.Errors
{
    public class BearerTokenMissing : IError
    {
        public string GetDescription() => "Expected header Authorization: Bearer <Authorization>";
    }
}
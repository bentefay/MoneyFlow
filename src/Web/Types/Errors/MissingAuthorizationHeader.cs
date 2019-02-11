namespace Web.Types.Errors
{
    public class MissingAuthorizationHeader : IError
    {
        public string GetDescription() => "Expected header Authorization: Bearer <Authorization>";
    }
}
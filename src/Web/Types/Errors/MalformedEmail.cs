namespace Web.Types.Errors
{
    public class MalformedEmail : IError, IDeserializeUserErrors, IParseAuthorizationErrors
    {
        public string Email { get; }

        public MalformedEmail(string email)
        {
            Email = email;
        }

        public string GetDescription() => $"Email '{Email}' is badly formatted";
    }
}
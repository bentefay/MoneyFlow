namespace Web.Types.Dtos
{
    public class AuthValidationErrors
    {
        public AuthValidationErrors(string[] email = null)
        {
            Email = email;
        }

        public string[] Email { get; }
    }
}
namespace Web.Types.Errors
{
    public class InvalidEmail : IError 
    {
        private readonly string _email;

        public InvalidEmail(string email)
        {
            _email = email;
        }

        public string GetDescription() => $"Email '{_email}' is invalid";
    }
}
namespace Web.Types.Errors
{
    public class InvalidHashedPassword : IError 
    {
        private readonly string _hashedPassword;

        public InvalidHashedPassword(string hashedPassword)
        {
            _hashedPassword = hashedPassword;
        }

        public string GetDescription() => $"Hashed password '{_hashedPassword}' is invalid";
    }
}
using Web.Functions;

namespace Web.Types.Errors
{
    public class InvalidHashedPassword : IError, IGetBlobTextErrors, IDeserializeVaultIndexErrors, IParseAuthorizationErrors
    {
        private readonly string _hashedPassword;

        public InvalidHashedPassword(string hashedPassword)
        {
            _hashedPassword = hashedPassword;
        }

        public string GetDescription() => $"Hashed password '{_hashedPassword}' is invalid";
    }
}
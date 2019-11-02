using Web.Functions;

namespace Web.Types.Errors
{
    public class MalformedPassword : IError, IGetBlobTextErrors, IDeserializeVaultIndexErrors, IParseAuthorizationErrors
    {
        public string HashedPassword { get; }
        
        public MalformedPassword(string hashedPassword)
        {
            HashedPassword = hashedPassword;
        }

        public string GetDescription() => $"Hashed password is badly formatted";
    }
}
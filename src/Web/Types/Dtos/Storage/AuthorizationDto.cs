namespace Web.Types.Dtos.Storage
{
    public class AuthorizationDto
    {
        public AuthorizationDto(string email, string hashedPassword)
        {
            Email = email;
            HashedPassword = hashedPassword;
        }

        public string Email { get; }
        
        public string HashedPassword { get; }
    }
}
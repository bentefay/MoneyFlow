namespace Web.Types.Dtos.Storage
{
    public class VaultIndexDto
    {
        public VaultIndexDto(string userId, string email, string passwordSalt, string password)
        {
            UserId = userId;
            Email = email;
            PasswordSalt = passwordSalt;
            Password = password;
        }

        public string UserId { get; }
        public string Email { get; }
        public string PasswordSalt { get; }
        public string Password { get; }
    }
}
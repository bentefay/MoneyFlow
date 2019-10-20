namespace Web.Types.Dtos
{
    public class VaultIndexDto
    {
        public VaultIndexDto(string email, string passwordSalt, string password)
        {
            Email = email;
            PasswordSalt = passwordSalt;
            Password = password;
        }

        public string Email { get; }
        public string PasswordSalt { get; }
        public string Password { get; }
    }
}
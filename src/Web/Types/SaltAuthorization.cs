namespace Web.Controllers
{
    public class SaltAuthorization
    {
        public SaltAuthorization(string email)
        {
            Email = email;
        }

        public string Email { get; }
    }
}
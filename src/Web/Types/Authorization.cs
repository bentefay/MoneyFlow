using Web.Types.Values;

namespace Web.Types
{
    public class Authorization
    {
        public Authorization(Email email, HashedPassword password)
        {
            Email = email;
            Password = password;
        }

        public Email Email { get; }

        public HashedPassword Password { get; }
    }
}
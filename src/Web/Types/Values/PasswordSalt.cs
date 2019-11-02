namespace Web.Types.Values
{
    public class PasswordSalt : TinyType<PasswordSalt, string>
    {
        public PasswordSalt(string value) : base(value)
        {
        }
    }
}
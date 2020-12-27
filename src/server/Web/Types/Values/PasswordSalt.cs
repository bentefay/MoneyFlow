namespace Web.Types.Values
{
    public record PasswordSalt(string Value) : ITinyType<string>;
}
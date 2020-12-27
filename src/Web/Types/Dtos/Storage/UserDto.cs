namespace Web.Types.Dtos.Storage
{
    public record UserDto(
        string UserId,
        string Email,
        string PasswordSalt,
        string Password
    );
}
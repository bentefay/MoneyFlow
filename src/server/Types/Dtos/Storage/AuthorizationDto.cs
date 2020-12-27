namespace Web.Types.Dtos.Storage
{
    public record AuthorizationDto(
        string Email,
        string HashedPassword
    );
}
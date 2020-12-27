namespace Web.Types.Dtos.Web
{
    public record UpdateVaultRequest(string UserId, string Content, string? ETag);
}
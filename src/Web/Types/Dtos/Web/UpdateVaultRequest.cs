namespace Web.Types.Dtos.Web
{
    public class UpdateVaultRequest
    {
        public string UserId { get; }
        public string Content { get; }
        public string? ETag { get; }

        public UpdateVaultRequest(string userId, string content, string? eTag)
        {
            UserId = userId;
            Content = content;
            ETag = eTag;
        }
    }
}
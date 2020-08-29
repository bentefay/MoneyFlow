namespace Web.Types.Dtos.Web
{
    public class GetVaultResponse
    {
        public string UserId { get; }
        public string Content { get; }
        public string ETag { get; }

        public GetVaultResponse(string userId, string content, string eTag)
        {
            UserId = userId;
            Content = content;
            ETag = eTag;
        }
    }
}
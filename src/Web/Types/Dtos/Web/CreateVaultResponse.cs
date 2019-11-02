namespace Web.Types.Dtos.Web
{
    public class CreateVaultResponse
    {
        public string UserId { get; }

        public CreateVaultResponse(string userId)
        {
            UserId = userId;
        }
    }
}
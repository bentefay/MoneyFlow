namespace Web.Types.Dtos.Web
{
    public class CreateUserResponse
    {
        public string UserId { get; }

        public CreateUserResponse(string userId)
        {
            UserId = userId;
        }
    }
}
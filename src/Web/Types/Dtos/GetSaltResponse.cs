namespace Web.Types.Dtos
{
    public class GetSaltResponse : IGetSaltResponse
    {
        public GetSaltResponse(string salt)
        {
            Salt = salt;
        }

        public string Salt { get; }
        
        public string Type => nameof(GetSaltResponse);
    }
}
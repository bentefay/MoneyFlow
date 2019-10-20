namespace Web.Types.Dtos
{
    public class GetSaltValidationErrorResponse : IGetSaltResponse
    {
        public GetSaltValidationErrorResponse(AuthValidationErrors validationErrors)
        {
            ValidationErrors = validationErrors;
        }
        
        public AuthValidationErrors ValidationErrors { get; }
        
        public string Type => nameof(GetSaltValidationErrorResponse);
    }
}
namespace Web.Types.Errors
{
    public class InvalidUserId : IError, IDeserializeVaultIndexErrors, IUpdateVaultRequestToVaultErrors
    {
        private readonly string _userId;

        public InvalidUserId(string userId)
        {
            _userId = userId;
        }

        public string GetDescription() => $"User ID '{_userId}' is invalid";
    }
}
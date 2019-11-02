namespace Web.Types.Errors
{
    public class MalformedUserId : IError, IDeserializeVaultIndexErrors, IUpdateVaultRequestToVaultErrors
    {
        private readonly string _userId;

        public MalformedUserId(string userId)
        {
            _userId = userId;
        }

        public string GetDescription() => $"User ID '{_userId}' is badly formatted";
    }
}
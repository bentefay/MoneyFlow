namespace Web.Types.Errors
{
    public class VaultAlreadyExists : IError
    {
        private readonly Email _email;

        public VaultAlreadyExists(Email email)
        {
            _email = email;
        }

        public string GetDescription() => $"Vault for '{_email}' already exists";
    }
}
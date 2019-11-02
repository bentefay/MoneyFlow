using LanguageExt;
using Web.Types;
using Web.Types.Errors;
using Web.Types.Values;
using Web.Utils.Extensions;

namespace Web.Functions
{
    public static class VaultIndexFunctions
    {
        public static Either<ICreateVaultIndexErrors, VaultIndex> CreateVaultIndex(Authorization authorization)
        {
            return
                from salt in CryptoFunctions.CreatePasswordSalt().Left(Cast.To<ICreateVaultIndexErrors>())
                let userId = UserId.Create()
                from password in CryptoFunctions.HashPassword(authorization.Password, salt).Left(Cast.To<ICreateVaultIndexErrors>())
                select new VaultIndex(userId, authorization.Email, salt, password);
        }
    }
}
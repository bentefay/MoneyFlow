using LanguageExt;
using Web.Types;
using Web.Types.Dtos.Web;
using Web.Types.Errors;
using Web.Types.Values;
using Web.Utils.Extensions;

namespace Web.Functions
{
    public static class VaultFunctions
    {
        public static EitherAsync<IUpdateVaultErrors, Unit> UpdateVault(string authorizationHeader, UpdateVaultRequest request, StorageConnectionString connectionString)
        {
            return
                from vault in MappingFunctions.FromDto(request).ToAsync().Left(Cast.To<IUpdateVaultErrors>())
                from authorization in AuthorizationFunctions.ParseAuthorization(authorizationHeader).ToAsync().Left(Cast.To<IUpdateVaultErrors>())
                from user in UserStorageFunctions.LoadUser(authorization.Email, connectionString).Left(Cast.To<IUpdateVaultErrors>())
                    .Bind(maybeUser =>
                        maybeUser.Match(
                            Some: Prelude.RightAsync<IUpdateVaultErrors, User>,
                            None: () => Prelude.LeftAsync<IUpdateVaultErrors, User>(new UserDoesNotExistError(authorization.Email))))
                from _ in AssertVaultAccess(authorization, user).ToAsync().Left(Cast.To<IUpdateVaultErrors>())
                from __ in user.UserId != vault.UserId ?
                    Prelude.LeftAsync<IUpdateVaultErrors, Unit>(new UserIdMismatchError(authorization.Email, vault.UserId, user.UserId)) :
                    Prelude.unit
                from ___ in VaultStorageFunctions.UpdateVault(vault, connectionString).Left(Cast.To<IUpdateVaultErrors>())
                select Prelude.unit;
        }

        public static EitherAsync<IGetVaultErrors, GetVaultResponse> GetVault(string authorizationHeader, StorageConnectionString connectionString)
        {
            return
                from authorization in AuthorizationFunctions.ParseAuthorization(authorizationHeader).ToAsync().Left(Cast.To<IGetVaultErrors>())
                from user in UserStorageFunctions.LoadUser(authorization.Email, connectionString).Left(Cast.To<IGetVaultErrors>())
                    .Bind(maybeUser =>
                        maybeUser.Match(
                            Some: Prelude.RightAsync<IGetVaultErrors, User>,
                            None: () => Prelude.LeftAsync<IGetVaultErrors, User>(new UserDoesNotExistError(authorization.Email))))
                from _ in AssertVaultAccess(authorization, user).ToAsync().Left(Cast.To<IGetVaultErrors>())
                from vault in VaultStorageFunctions.LoadVault(user.UserId, connectionString).Left(Cast.To<IGetVaultErrors>())
                    .Bind(maybeVault =>
                        maybeVault.Match(
                            Some: Prelude.RightAsync<IGetVaultErrors, TaggedVault>,
                            None: () => Prelude.LeftAsync<IGetVaultErrors, TaggedVault>(new VaultDoesNotExistError(authorization.Email, user.UserId))))
                select MappingFunctions.ToDto(vault);
        }

        public static Either<IAssertVaultAccessErrors, Unit> AssertVaultAccess(Authorization authorization, User user)
        {
            return
                from password in CryptoFunctions.HashPassword(authorization.Password, user.PasswordSalt).Left(Cast.To<IAssertVaultAccessErrors>())
                from _ in user.Email == authorization.Email ?
                    Prelude.unit :
                    Prelude.Left<IAssertVaultAccessErrors, Unit>(new EmailIncorrectError(user.Email, authorization.Email))
                from __ in user.Password == password ?
                    Prelude.unit :
                    Prelude.Left<IAssertVaultAccessErrors, Unit>(new PasswordIncorrectError(authorization.Email))
                select Prelude.unit;
        }
    }
}
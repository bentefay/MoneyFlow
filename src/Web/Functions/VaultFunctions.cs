using LanguageExt;
using Web.Controllers;
using Web.Types;
using Web.Types.Dtos;
using Web.Types.Dtos.Web;
using Web.Types.Errors;
using Web.Types.Values;
using Web.Utils.Extensions;

namespace Web.Functions
{
    public static class VaultFunctions
    {
        public static EitherAsync<ICreateVaultErrors, CreateVaultResponse> CreateVault(string authorizationHeader, StorageConnectionString connectionString)
        {
            return
                from authorization in AuthorizationFunctions.ParseAuthorization(authorizationHeader).ToAsync().Left(Cast.To<ICreateVaultErrors>())
                from vaultIndex in VaultIndexFunctions.CreateVaultIndex(authorization).ToAsync().Left(Cast.To<ICreateVaultErrors>())
                from _ in VaultStorageFunctions.CreateVaultIndex(vaultIndex, connectionString).Left(Cast.To<ICreateVaultErrors>())
                select MappingFunctions.ToDto(vaultIndex.UserId);
        }
        
        public static EitherAsync<IUpdateVaultErrors, Unit> UpdateVault(string authorizationHeader, UpdateVaultRequest request, StorageConnectionString connectionString)
        {
            return
                from vault in MappingFunctions.FromDto(request).ToAsync().Left(Cast.To<IUpdateVaultErrors>())
                from authorization in AuthorizationFunctions.ParseAuthorization(authorizationHeader).ToAsync().Left(Cast.To<IUpdateVaultErrors>())
                from vaultIndex in VaultStorageFunctions.LoadVaultIndex(authorization.Email, connectionString).Left(Cast.To<IUpdateVaultErrors>())
                    .Bind(maybeVaultIndex =>
                        maybeVaultIndex.Match(
                            Some: Prelude.RightAsync<IUpdateVaultErrors, VaultIndex>,
                            None: () => Prelude.LeftAsync<IUpdateVaultErrors, VaultIndex>(new VaultIndexDoesNotExist(authorization.Email))))
                from _ in AssertVaultAccess(authorization, vaultIndex).ToAsync().Left(Cast.To<IUpdateVaultErrors>())
                from __ in vaultIndex.UserId != vault.UserId ? 
                    Prelude.LeftAsync<IUpdateVaultErrors, Unit>(new UserIdMismatchError(authorization.Email, vault.UserId, vaultIndex.UserId)) : 
                    Prelude.unit
                from ___ in VaultStorageFunctions.UpdateVault(vault, connectionString).Left(Cast.To<IUpdateVaultErrors>())
                select Prelude.unit;
        }
        
        public static EitherAsync<IGetVaultErrors, GetVaultResponse> GetVault(string authorizationHeader, StorageConnectionString connectionString)
        {
            return
                from authorization in AuthorizationFunctions.ParseAuthorization(authorizationHeader).ToAsync().Left(Cast.To<IGetVaultErrors>())
                from vaultIndex in VaultStorageFunctions.LoadVaultIndex(authorization.Email, connectionString).Left(Cast.To<IGetVaultErrors>())
                    .Bind(maybeVaultIndex =>
                        maybeVaultIndex.Match(
                            Some: Prelude.RightAsync<IGetVaultErrors, VaultIndex>,
                            None: () => Prelude.LeftAsync<IGetVaultErrors, VaultIndex>(new VaultIndexDoesNotExist(authorization.Email))))
                from _ in AssertVaultAccess(authorization, vaultIndex).ToAsync().Left(Cast.To<IGetVaultErrors>())
                from vault in VaultStorageFunctions.LoadVault(vaultIndex.UserId, connectionString).Left(Cast.To<IGetVaultErrors>())
                    .Bind(maybeVault =>
                        maybeVault.Match(
                            Some: Prelude.RightAsync<IGetVaultErrors, TaggedVault>,
                            None: () => Prelude.LeftAsync<IGetVaultErrors, TaggedVault>(new VaultDoesNotExistError(authorization.Email, vaultIndex.UserId))))
                select MappingFunctions.ToDto(vault);
        }

        public static Either<IAssertVaultAccessErrors, Unit> AssertVaultAccess(Authorization authorization, VaultIndex vaultIndex)
        {
            return
                from password in CryptoFunctions.HashPassword(authorization.Password, vaultIndex.PasswordSalt).Left(Cast.To<IAssertVaultAccessErrors>())
                from _ in vaultIndex.Email == authorization.Email ? 
                    Prelude.Left<IAssertVaultAccessErrors, Unit>(new EmailIncorrectError(authorization.Email)) : 
                    Prelude.unit
                from __ in vaultIndex.Password == password ? 
                    Prelude.Left<IAssertVaultAccessErrors, Unit>(new PasswordIncorrectError(authorization.Email)) : 
                    Prelude.unit
                select Prelude.unit;
        }
    }
}
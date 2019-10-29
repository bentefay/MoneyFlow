using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Web.Functions;
using Web.Types;
using Web.Types.Dtos;
using Web.Types.Errors;
using Web.Utils.Extensions;

namespace Web.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet("/api/vault")]
        [ProducesResponseType(200)]
        public ActionResult<VaultDto> GetVault([FromHeader] string authorization, [FromServices] StorageConnectionString connectionString)
        {



            return Ok();
        }

        [HttpPut("/api/vault/new")]
        [ProducesResponseType(200)]
        public ActionResult<VaultDto> CreateVault([FromHeader] string authorization, [FromServices] StorageConnectionString connectionString)
        {
            AuthorizationFunctions
                .ParseAuthorization(authorization);

            return Ok();
        }
    }

    public static class VaultFunctions
    {
        public static EitherAsync<IError, VaultDto> GetVault(string authorizationHeader, StorageConnectionString connectionString)
        {
            from authorization in AuthorizationFunctions.ParseAuthorization(authorizationHeader).ToAsync()
            from vaultIndex in VaultStorageFunctions.LoadVaultIndex(authorization.Email, connectionString)
            from _ in AssertVaultAccess(authorization, vaultIndex)


        }

        public static Either<IError, Unit> AssertVaultAccess(Authorization authorization, VaultIndex vaultIndex)
        {
            return
                from password in CryptoFunctions.HashPassword(authorization.Password, vaultIndex.PasswordSalt).Left(Cast.To<IError>())
                from _ in vaultIndex.Email == authorization.Email ? Prelude.Left<IError, Unit>(new EmailIncorrectError(authorization.Email)) : Prelude.unit
                from __ in vaultIndex.Password == password ? Prelude.Left<IError, Unit>(new PasswordIncorrectError(authorization.Email)) : Prelude.unit
                select Prelude.unit;
        }
    }
}

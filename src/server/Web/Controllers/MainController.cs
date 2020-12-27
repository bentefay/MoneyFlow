using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Web.Functions;
using Web.Types.Dtos.Web;
using Web.Types.Errors;
using Web.Types.Values;
using Web.Utils.Extensions;

namespace Web.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class MainController : ControllerBase
    {
        private readonly ILogger _logger;

        public MainController(ILogger logger)
        {
            _logger = logger;
        }

        [HttpPut("/api/users")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Task<ActionResult<CreateUserResponse>> CreateUser([FromHeader] string authorization, [FromServices] StorageConnectionString connectionString)
        {
            return UserFunctions.CreateUser(authorization, connectionString)
                .DoLeft(LoggerFunctions.LogControllerError(_logger))
                .Match<ActionResult<CreateUserResponse>>(
                    Right: vaultResponse => CreatedAtAction("GetVault", vaultResponse),
                    Left: error => error.Match(
                        bearerTokenMissing: HandleBadRequest,
                        malformedBearerToken: HandleBadRequest,
                        malformedEmail: HandleBadRequest,
                        malformedPassword: HandleBadRequest,
                        base64Decode: HandleBadRequest,

                        couldNotCreateBlobBecauseItAlreadyExists: HandleConflict,

                        couldNotUpdateBlobBecauseTheETagHasChanged: HandleServerError,
                        generalStorage: HandleServerError,
                        generateSalt: HandleServerError,
                        hashPassword: HandleServerError,
                        jsonDeserialization: HandleServerError,
                        jsonSerialization: HandleServerError,
                        malformedCloudStorageConnectionString: HandleServerError,
                        malformedETag: HandleServerError
                    ));

            ActionResult HandleBadRequest(IError error) => BadRequest(error.GetDescription());
            ActionResult HandleConflict(IError _) => Conflict("An account with that email already exists");
            ActionResult HandleServerError(IError _) => StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpGet("/api/vaults")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Task<ActionResult<GetVaultResponse>> GetVault([FromHeader] string authorization, [FromServices] StorageConnectionString connectionString)
        {
            return VaultFunctions.GetVault(authorization, connectionString)
                .DoLeft(LoggerFunctions.LogControllerError(_logger))
                .Match<ActionResult<GetVaultResponse>>(
                    Right: vaultResponse => Ok(vaultResponse),
                    Left: error => error.Match(
                        bearerTokenMissing: HandleBadRequest,
                        malformedBearerToken: HandleBadRequest,
                        malformedEmail: HandleBadRequest,
                        malformedPassword: HandleBadRequest,
                        base64Decode: HandleBadRequest,
                        jsonDeserialization: HandleBadRequest,

                        emailIncorrect: HandleUnauthorized,
                        passwordIncorrect: HandleUnauthorized,
                        userDoesNotExist: HandleUnauthorized,

                        vaultDoesNotExist: HandleVaultDoesNotExistYet,

                        generalStorage: HandleServerError,
                        hashPassword: HandleServerError,
                        malformedCloudStorageConnectionString: HandleServerError,
                        malformedETag: HandleServerError,
                        malformedUserId: HandleServerError,
                        userIdMismatch: HandleServerError
                        ));

            ActionResult HandleBadRequest(IError error) => BadRequest(error.GetDescription());
            ActionResult HandleVaultDoesNotExistYet(VaultDoesNotExistError error) => NotFound(new CreateUserResponse(error.UserId.Value.ToString()));
            ActionResult HandleUnauthorized(IError _) => Unauthorized("Either your email or password are incorrect, or no user exists with that email");
            ActionResult HandleServerError(IError _) => StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpPut("/api/vaults")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Task<ActionResult> UpdateVault([FromHeader] string authorization, [FromBody] UpdateVaultRequest request, [FromServices] StorageConnectionString connectionString)
        {
            return VaultFunctions.UpdateVault(authorization, request, connectionString)
                .DoLeft(LoggerFunctions.LogControllerError(_logger))
                .Match<ActionResult>(
                    Right: vaultResponse => Ok(vaultResponse),
                    Left: error => error.Match(
                        bearerTokenMissing: HandleBadRequest,
                        malformedBearerToken: HandleBadRequest,
                        malformedEmail: HandleBadRequest,
                        malformedETag: HandleBadRequest,
                        malformedPassword: HandleBadRequest,
                        base64Decode: HandleBadRequest,

                        userDoesNotExist: HandleUnauthorized,
                        emailIncorrect: HandleUnauthorized,
                        passwordIncorrect: HandleUnauthorized,

                        couldNotUpdateBlobBecauseTheETagHasChanged: _ =>
                            Conflict("The version sent with your vault was not for the latest saved version of the vault. " + OldVersionOfVaultError()),

                        couldNotCreateBlobBecauseItAlreadyExists: _ =>
                            Conflict("No version was sent with your vault, but the vault already exists. " + OldVersionOfVaultError()),

                        generalStorage: HandleServerError,
                        hashPassword: HandleServerError,
                        jsonDeserialization: HandleServerError,
                        jsonSerialization: HandleServerError,
                        malformedCloudStorageConnectionString: HandleServerError,
                        malformedUserId: HandleServerError,
                        userIdMismatch: HandleServerError,
                        vaultDoesNotExist: HandleServerError
                    ));

            ActionResult HandleBadRequest(IError error) => BadRequest(error.GetDescription());
            ActionResult HandleUnauthorized(IError error) => Unauthorized("Either your email or password are incorrect, or no user exists with that email");
            ActionResult HandleServerError(IError _) => StatusCode(StatusCodes.Status500InternalServerError);

            string OldVersionOfVaultError() => "This means you were editing an old version. This usually happens when you were editing" +
                                               "the vault in more than one browser at the same time. Refresh to get the " +
                                               "latest version. Unfortunately, this means you will lose your most recent changes. Sorry!";
        }
    }
}
using System.Net;
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
    public class VaultController : ControllerBase
    {
        private readonly ILogger _logger;

        public VaultController(ILogger logger)
        {
            _logger = logger;
        }
        
        [HttpGet("/api/vault")]
        [ProducesResponseType(200)]
        public Task<ActionResult<GetVaultResponse>> GetVault([FromHeader] string authorization, [FromServices] StorageConnectionString connectionString)
        {
            return VaultFunctions.GetVault(authorization, connectionString)
                .DoLeft(LoggerFunctions.LogControllerError(_logger))
                .Match<ActionResult<GetVaultResponse>>(
                    Right: vaultResponse => Ok(vaultResponse),
                    Left: error => {
                        switch (error)
                        {
                            case BearerTokenMissing _:
                            case MalformedBearerToken _:
                            case MalformedEmail _:
                            case MalformedPassword _:
                            case Base64DecodeError _:
                            case JsonDeserializationError _:
                                return BadRequest(error.GetDescription());
                            case EmailIncorrectError _:
                            case PasswordIncorrectError _:
                            case VaultIndexDoesNotExist _:
                                return Unauthorized("Either your email or password are incorrect, or no user exists with that email");
                            case GeneralStorageError _:
                            case HashPasswordError _:
                            case MalformedCloudStorageConnectionString _:
                            case MalformedETag _:
                            case MalformedUserId _:
                            case UserIdMismatchError _:
                            case VaultDoesNotExistError _:
                                return StatusCode(StatusCodes.Status500InternalServerError);
                            default:
                                return StatusCode(StatusCodes.Status500InternalServerError);
                        }
                    });
        }

        [HttpPut("/api/vault/new")]
        [ProducesResponseType(200)]
        public Task<ActionResult<CreateVaultResponse>> CreateVault([FromHeader] string authorization, [FromServices] StorageConnectionString connectionString)
        {
            return VaultFunctions.CreateVault(authorization, connectionString)
                .DoLeft(LoggerFunctions.LogControllerError(_logger))
                .Match<ActionResult<CreateVaultResponse>>(
                    Right: vaultResponse => Ok(vaultResponse),
                    Left: error => {
                        switch (error)
                        {
                            case BearerTokenMissing _:
                            case MalformedBearerToken _:
                            case MalformedEmail _:
                            case MalformedPassword _:
                            case Base64DecodeError _:
                                return BadRequest(error.GetDescription());
                            case CouldNotCreateBlobBecauseItAlreadyExistsError _:
                                return Conflict("An account with that email already exists");
                            case CouldNotUpdateBlobBecauseTheETagHasChanged _:
                            case GeneralStorageError _:
                            case GenerateSaltError _:
                            case HashPasswordError _:
                            case JsonDeserializationError _:
                            case JsonSerializationError _:
                            case MalformedCloudStorageConnectionString _:
                            case MalformedETag _:
                                return StatusCode(StatusCodes.Status500InternalServerError);
                            default: 
                                return StatusCode(StatusCodes.Status500InternalServerError);
                                
                        }
                    });
        }
        
        [HttpPut("/api/vault")]
        [ProducesResponseType(200)]
        public Task<ActionResult> UpdateVault([FromHeader] string authorization, [FromBody] UpdateVaultRequest request, [FromServices] StorageConnectionString connectionString)
        {
            return VaultFunctions.UpdateVault(authorization, request, connectionString)
                .DoLeft(LoggerFunctions.LogControllerError(_logger))
                .Match<ActionResult>(
                    Right: vaultResponse => Ok(vaultResponse),
                    Left: error => {
                        switch (error)
                        {
                            case BearerTokenMissing _:
                            case MalformedBearerToken _:
                            case MalformedEmail _:
                            case MalformedETag _:
                            case MalformedPassword _:
                            case Base64DecodeError _:
                                return BadRequest(error.GetDescription());
                            case VaultIndexDoesNotExist _:
                            case EmailIncorrectError _:
                            case PasswordIncorrectError _:
                                return Unauthorized("Either your email or password are incorrect, or no user exists with that email");
                            case CouldNotUpdateBlobBecauseTheETagHasChanged _:
                                return Conflict("The version sent with your vault was not for the latest saved version of the vault. " +
                                                OldVersionOfVaultError);
                            case CouldNotCreateBlobBecauseItAlreadyExistsError _:
                                return Conflict("No version was sent with your vault, but the vault already exists. " + 
                                                OldVersionOfVaultError);
                            case GeneralStorageError _:
                            case HashPasswordError _:
                            case JsonDeserializationError _:
                            case JsonSerializationError _:
                            case MalformedCloudStorageConnectionString _:
                            case MalformedUserId _:
                            case UserIdMismatchError _:
                            case VaultDoesNotExistError _:
                                return StatusCode(StatusCodes.Status500InternalServerError);
                            default:
                                return StatusCode(StatusCodes.Status500InternalServerError);
                        }
                    });
        }

        private const string OldVersionOfVaultError = "This means you were editing an old version. This usually happens when you were editing" +
                                                      "the vault in more than one browser at the same time. Refresh to get the " +
                                                      "latest version. Unfortunately, this means you will lose your most recent changes. Sorry!";
    }
}

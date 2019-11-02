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
                                return BadRequest(error.GetDescription());
                            case EmailIncorrectError _:
                            case PasswordIncorrectError _:
                            case VaultIndexDoesNotExist _:
                                return Unauthorized("Either your email or password are incorrect, or no user exists with that email");
                            case GeneralStorageError generalStorageError:
                            case HashPasswordError hashPasswordError:
                            case Base64DecodeError base64DecodeError:
                            case JsonDeserializationError jsonDeserializationError:
                            case MalformedCloudStorageConnectionString malformedCloudStorageConnectionString:
                            case MalformedETag malformedETag:
                            case MalformedUserId malformedUserId:
                            case UserIdMismatchError userIdMismatchError:
                            case VaultDoesNotExistError vaultDoesNotExistError:
                                return StatusCode(StatusCodes.Status500InternalServerError);
                            default:
                                return StatusCode(StatusCodes.Status500InternalServerError);
                        }
                    });
        }

        [HttpPut("/api/vault/new")]
        [ProducesResponseType(200)]
        public ActionResult<CreateVaultResponse> CreateVault([FromHeader] string authorization, [FromServices] StorageConnectionString connectionString)
        {
            VaultFunctions.CreateVault(authorization, connectionString);
            return Ok();
        }
        
        [HttpPut("/api/vault")]
        [ProducesResponseType(200)]
        public ActionResult UpdateVault([FromHeader] string authorization, [FromBody] UpdateVaultRequest request, [FromServices] StorageConnectionString connectionString)
        {
            VaultFunctions.UpdateVault(authorization, request, connectionString);
            return Ok();
        }
    }
}

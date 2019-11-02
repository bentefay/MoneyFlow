using Microsoft.AspNetCore.Mvc;
using Serilog;
using Web.Functions;
using Web.Types.Dtos.Web;
using Web.Types.Values;

namespace Web.Controllers
{
    [ApiController]
    public class VaultController : ControllerBase
    {
        [HttpGet("/api/vault")]
        [ProducesResponseType(200)]
        public ActionResult<GetVaultResponse> GetVault([FromHeader] string authorization, [FromServices] StorageConnectionString connectionString, [FromServices] ILogger logger)
        {
            VaultFunctions.GetVault(authorization, connectionString);
            return Ok();
        }

        [HttpPut("/api/vault/new")]
        [ProducesResponseType(200)]
        public ActionResult<CreateVaultResponse> CreateVault([FromHeader] string authorization, [FromServices] StorageConnectionString connectionString, [FromServices] ILogger logger)
        {
            VaultFunctions.CreateVault(authorization, connectionString);
            return Ok();
        }
        
        [HttpPut("/api/vault")]
        [ProducesResponseType(200)]
        public ActionResult UpdateVault([FromHeader] string authorization, [FromBody] UpdateVaultRequest request, [FromServices] StorageConnectionString connectionString, [FromServices] ILogger logger)
        {
            VaultFunctions.UpdateVault(authorization, request, connectionString);
            return Ok();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Web.Functions;
using Web.Types.Dtos;

namespace Web.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet("/api/vault")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(AuthValidationErrors))]
        public ActionResult<GetSaltResponse> GetVault([FromHeader] string authorization)
        {
            AuthorizationFunctions
                .ParseAuthorization(authorization);
            
            return Ok(new GetSaltResponse(""));
        }
        
        [HttpPut("/api/vault/new")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(AuthValidationErrors))]
        public ActionResult<GetSaltResponse> PutVault([FromHeader] string authorization)
        {
            AuthorizationFunctions
                .ParseAuthorization(authorization);
            
            return Ok(new GetSaltResponse(""));
        }
    }
}

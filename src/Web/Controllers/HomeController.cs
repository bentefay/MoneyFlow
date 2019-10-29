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
        public ActionResult<GetSaltResponse> GetVault([FromHeader] string authorization)
        {
            AuthorizationFunctions
                .ParseAuthorization(authorization);
            
            return Ok(new GetSaltResponse(""));
        }
        
        [HttpPut("/api/vault/new")]
        [ProducesResponseType(200)]
        public ActionResult<GetSaltResponse> CreateVault([FromHeader] string authorization)
        {
            AuthorizationFunctions
                .ParseAuthorization(authorization);
            
            return Ok(new GetSaltResponse(""));
        }
    }
}

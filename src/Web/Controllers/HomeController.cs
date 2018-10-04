using System;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet("/api/salt")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(AuthValidationErrors))]
        public ActionResult<GetSaltResponse> GetSalt([FromHeader] string authorization)
        {           
            return Ok(new GetSaltResponse(""));
        }
    }

    public class GetSaltResponse
    {
        public GetSaltResponse(string salt)
        {
            Salt = salt;
        }

        public string Salt { get; }
    }
    
    public class GetSaltValidationErrorResponse
    {
        public GetSaltValidationErrorResponse(AuthValidationErrors validationErrors)
        {
            ValidationErrors = validationErrors;
        }
        
        public AuthValidationErrors ValidationErrors { get; }
    }

    public class AuthValidationErrors
    {
        public AuthValidationErrors(string[] email = null)
        {
            Email = email;
        }

        public string[] Email { get; }
    }
}

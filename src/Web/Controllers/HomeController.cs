using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

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
    
    public interface IGetSaltResponse {}

    public class GetSaltResponse : IGetSaltResponse
    {
        public GetSaltResponse(string salt)
        {
            Salt = salt;
        }

        public string Salt { get; }
    }
    
    public class GetSaltValidationErrorResponse : IGetSaltResponse
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

    public class VaultRepository
    {
        public void GetVaultSummary(string authorization)
        {
            var storageConnectionString = "";
            var account = CloudStorageAccount.Parse(storageConnectionString);
            var serviceClient = account.CreateCloudBlobClient();
            var container = serviceClient.GetContainerReference("vaults");
            container.GetBlobReference(authorization);
        }
    }
}

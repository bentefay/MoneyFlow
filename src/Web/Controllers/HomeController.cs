using System;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Web.Functions;
using Web.Types;
using Web.Types.Errors;

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
        
        [HttpPut("/api/vault")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(AuthValidationErrors))]
        public ActionResult<GetSaltResponse> PutVault([FromHeader] string authorization, [FromBody] WrappedVault wrappedVault)
        {
            AuthorizationFunctions
                .ParseAuthorization(authorization);
            
            return Ok(new GetSaltResponse(""));
        }
    }

    public interface IGetSaltResponse : ISumType
    {
    }

    public class GetSaltResponse : IGetSaltResponse
    {
        public GetSaltResponse(string salt)
        {
            Salt = salt;
        }

        public string Salt { get; }
        
        public string Type => nameof(GetSaltResponse);
    }
    
    public class GetSaltValidationErrorResponse : IGetSaltResponse
    {
        public GetSaltValidationErrorResponse(AuthValidationErrors validationErrors)
        {
            ValidationErrors = validationErrors;
        }
        
        public AuthValidationErrors ValidationErrors { get; }
        
        public string Type => nameof(GetSaltValidationErrorResponse);
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
        public EitherAsync<IError, WrappedVault> GetVault(Authorization authorization)
        {
            GetVaultSummary(authorization);
            throw new Exception();
        }

        private static EitherAsync<IError, VaultSummary> GetVaultSummary(Authorization authorization)
        {
            var storageConnectionString = "";
            var account = CloudStorageAccount.Parse(storageConnectionString);
            var serviceClient = account.CreateCloudBlobClient();
            var container = serviceClient.GetContainerReference("vaults");
            container.GetBlobReference(authorization.Email.Value);
            throw new Exception();
        }

        public static void PutVault(Authorization authorization, WrappedVault wrappedVault)
        {
            var storageConnectionString = "";
            var account = CloudStorageAccount.Parse(storageConnectionString);
            var serviceClient = account.CreateCloudBlobClient();
            var container = serviceClient.GetContainerReference("vaults");
            var blobReference = container.Async(authorization.Email.Value);
            blobReference.
            throw new Exception();
        }
    }

    public class VaultSummary
    {
        public VaultSummary(Email email, PasswordSalt passwordSalt, SaltedHashedPassword password)
        {
            Email = email;
            PasswordSalt = passwordSalt;
            Password = password;
        }

        public Email Email { get; }
        public PasswordSalt PasswordSalt { get; }
        public SaltedHashedPassword Password { get; }
    }

    public class SaltedHashedPassword : ITinyType<string>
    {
        public SaltedHashedPassword(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
    
    public class PasswordSalt : ITinyType<string>
    {
        public PasswordSalt(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }

    public class WrappedVault
    {
        public ulong Version { get; }
        public Base64EncryptedVault Vault { get; }

        public WrappedVault(ulong version, Base64EncryptedVault vault)
        {
            Version = version;
            Vault = vault;
        }
    }

    public struct Base64EncryptedVault : ITinyType<string>
    {
        public string Value { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

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

    public static class Authorization
    {
        public static JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};
        
        public static Either<IError, SaltAuthorization> ParseSaltAuthorization(string authorization)
        {
            var tokens = authorization.Split(" ");

            if (tokens.Length != 2 || tokens[0] != "Bearer")
                return new ClientError(HttpStatusCode.Unauthorized, "Expected header Authorization: Bearer <Authorization>");

            var base64Json = tokens[1];
            
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(base64Json));
            
            var saltAuthorization = JsonConvert.DeserializeObject<SaltAuthorization>(json, JsonSerializerSettings);
           
            return saltAuthorization;
        }
    }

    public static class Json
    {
        public static Either<JsonDeserializationError, T> Deserialize<T>(string json, JsonSerializerSettings settings)
        {
            try
            {
                settings.Error += (sender, args) =>
                {
                }
                
                return JsonConvert.DeserializeObject<T>(json, settings );
            }
            catch (JsonReaderException e)
            {
                return new JsonDeserializationError(json, e.Path, e.LineNumber, e.LinePosition, e.Message);
            }
        }
        
        public static Either<string, T> Serialize<T>(T value, JsonSerializerSettings settings)
        {
            
        }
    }
   
    public class JsonDeserializationError : IError
    {
        public string Json { get; }
        public string Path { get; }
        public int LineNumber { get; }
        public int LinePosition { get; }
        public string Message { get; }

        public JsonDeserializationError(string json, string path, int lineNumber, int linePosition, string message)
        {
            Json = json;
            Path = path;
            LineNumber = lineNumber;
            LinePosition = linePosition;
            Message = message;
        }

        public string GetDescription()
        {
            var annotatedJson = AnnotateJson(Json, LineNumber, LinePosition);
            return $"Deserialization error at path '{Path}({LineNumber},{LinePosition}): {Message}\n\n{annotatedJson}'";
        }

        private static string AnnotateJson(string json, int lineNumber, int linePosition)
        {
            var lines = json.Split(Environment.NewLine).ToList();
            
            if (lineNumber < 1 || lineNumber > lines.Count)
                return json;

            var lineToAnnotate = lines[lineNumber - 1];

            if (linePosition < 1 || linePosition > lineToAnnotate.Length)
            {
                lines.Insert(linePosition, new string('^', lineToAnnotate.Length));
            }
            else
            {
                lines.Insert(linePosition, new string(' ', linePosition - 1) + "^");                
            }

            var contextLines = 20;
            
            return lines
                .Skip(Math.Max(0, linePosition - contextLines))
                .Take(2 * contextLines)
                .Join(Environment.NewLine);
        } 
    }

    public static class EnumerableExtensions
    {
        public static string Join(this IEnumerable<string> items, string separator)
        {
            return string.Join(separator, items);
        }
    }
    
    public class JsonSerializationError : IError
    {
        public string GetDescription();
    }
    
    public class SaltAuthorization
    {
        public SaltAuthorization(string email)
        {
            Email = email;
        }

        public string Email { get; }
    }
    
    public interface IError
    {
        string GetDescription();
    }

    public class VaultRepository
    {
        public void GetVaultSummary(string authorization)
        {
            var storageConnectionString = "";
            var account = CloudStorageAccount.Parse(storageConnectionString);
            var serviceClient = account.CreateCloudBlobClient();
            var container = serviceClient.GetContainerReference("vaults");
            container.GetBlobReference(authorization)
        }
    }
}

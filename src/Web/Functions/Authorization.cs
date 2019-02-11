using System;
using System.Text;
using LanguageExt;
using Web.Functions;
using Web.Types.Errors;
using Web.Utils.Extensions;
using Web.Utils.Serialization.Serializers;

namespace Web.Controllers
{
    public static class Authorization
    {      
        public static Either<IError, SaltAuthorization> ParseSaltAuthorization(string authorization)
        {
            if (authorization == null)
                return new MissingAuthorizationHeader();
            
            var tokens = authorization.Split(" ");

            if (tokens.Length != 2 || tokens[0] != "Bearer")
                return new MalformedAuthorizationHeader(authorization);

            var base64Json = tokens[1];
            
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(base64Json));
            
            return Json
                .Deserialize<SaltAuthorization>(json, ApiSerializers.Instance)
                .Left(Cast.To<IError>());
        }
    }
}
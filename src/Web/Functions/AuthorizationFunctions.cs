using System;
using System.Text;
using LanguageExt;
using Web.Types;
using Web.Types.Dtos;
using Web.Types.Errors;
using Web.Utils.Extensions;
using Web.Utils.Serialization.Serializers;
using static Web.Utils.Extensions.Cast;

namespace Web.Functions
{
    public static class AuthorizationFunctions
    {      
        public static Either<IError, Authorization> ParseAuthorization(string authorization)
        {
            if (authorization == null)
                return new MissingAuthorizationHeader();
            
            var tokens = authorization.Split(" ");

            if (tokens.Length != 2 || tokens[0] != "Bearer")
                return new MalformedAuthorizationHeader(authorization);

            var base64Json = tokens[1];
            
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(base64Json));
            
            return 
                from dto in JsonFunctions.Deserialize<AuthorizationDto>(json, ApiSerializers.Instance).Left(To<IError>())
                from email in Email.Create(dto.Email).Left(To<IError>())
                from password in HashedPassword.Create(dto.HashedPassword).Left(To<IError>())
                select new Authorization(email, password);
        }
    }
}
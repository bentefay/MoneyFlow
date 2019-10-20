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
                return new BearerTokenMissing();

            var tokens = authorization.Split(" ");

            if (tokens.Length != 2 || tokens[0] != "Bearer")
                return new BearerTokenMalformed(authorization);

            var base64Json = tokens[1];

            return
                from json in Base64Functions.DecodeBase64(base64Json).Left(To<IError>())
                from dto in JsonFunctions.Deserialize<AuthorizationDto>(json, ApiSerializers.Instance).Left(To<IError>())
                from email in Email.Create(dto.Email).Left(To<IError>())
                from password in HashedPassword.Create(dto.HashedPassword).Left(To<IError>())
                select new Authorization(email, password);
        }

      
    }
}
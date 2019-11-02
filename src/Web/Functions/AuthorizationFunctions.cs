using LanguageExt;
using Web.Types;
using Web.Types.Dtos;
using Web.Types.Dtos.Storage;
using Web.Types.Errors;
using Web.Types.Values;
using Web.Utils.Extensions;
using Web.Utils.Serialization.Serializers;
using static Web.Utils.Extensions.Cast;

namespace Web.Functions
{
    public static class AuthorizationFunctions
    {
        public static Either<IParseAuthorizationErrors, Authorization> ParseAuthorization(string authorization)
        {
            if (authorization == null)
                return new BearerTokenMissing();

            var tokens = authorization.Split(" ");

            if (tokens.Length != 2 || tokens[0] != "Bearer")
                return new MalformedBearerToken(authorization);

            var base64Json = tokens[1];

            return
                from json in Base64Functions.DecodeBase64ToString(base64Json).Left(To<IParseAuthorizationErrors>())
                from dto in JsonFunctions.Deserialize<AuthorizationDto>(json, ApiSerializers.Instance).Left(To<IParseAuthorizationErrors>())
                from email in Email.Create(dto.Email).Left(To<IParseAuthorizationErrors>())
                from password in HashedPassword.Create(dto.HashedPassword).Left(To<IParseAuthorizationErrors>())
                select new Authorization(email, password);
        }


    }
}
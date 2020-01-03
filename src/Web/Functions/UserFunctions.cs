using LanguageExt;
using Web.Types;
using Web.Types.Dtos.Web;
using Web.Types.Errors;
using Web.Types.Values;
using Web.Utils.Extensions;

namespace Web.Functions
{
    public static class UserFunctions
    {
        public static EitherAsync<ICreateUserErrors, CreateUserResponse> CreateUser(string authorizationHeader, StorageConnectionString connectionString)
        {
            return
                from authorization in AuthorizationFunctions.ParseAuthorization(authorizationHeader).ToAsync().Left(Cast.To<ICreateUserErrors>())
                from user in BuildUser(authorization).ToAsync().Left(Cast.To<ICreateUserErrors>())
                from _ in UserStorageFunctions.CreateUser(user, connectionString).Left(Cast.To<ICreateUserErrors>())
                select MappingFunctions.ToDto(user.UserId);
        }
        
        private static Either<IBuilderUserErrors, User> BuildUser(Authorization authorization)
        {
            return
                from salt in CryptoFunctions.GeneratePasswordSalt().Left(Cast.To<IBuilderUserErrors>())
                let userId = UserId.Create()
                from password in CryptoFunctions.HashPassword(authorization.Password, salt).Left(Cast.To<IBuilderUserErrors>())
                select new User(userId, authorization.Email, salt, password);
        }
    }
}
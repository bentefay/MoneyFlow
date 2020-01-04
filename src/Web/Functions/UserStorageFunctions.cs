using System.Web;
using LanguageExt;
using Web.Types;
using Web.Types.Errors;
using Web.Types.Values;
using Web.Utils.Extensions;

namespace Web.Functions
{
    public static class UserStorageFunctions
    {
        private const string Container = "users";
        private static string GetIndexPath(Email email) => HttpUtility.UrlEncode(email.Value);
        
        public static EitherAsync<ISaveNewUserErrors, Unit> CreateUser(User user, StorageConnectionString connectionString)
        {
            return
                from blob in StorageFunctions.GetBlob(Container, GetIndexPath(user.Email), connectionString).ToAsync().Left(Cast.To<ISaveNewUserErrors>())
                from json in SerializationFunctions.SerializeUser(user).ToAsync().Left(Cast.To<ISaveNewUserErrors>())
                from _ in StorageFunctions.SetBlobText(blob, json).Left(Cast.To<ISaveNewUserErrors>())
                select Prelude.unit;
        }
        
        public static EitherAsync<IUpdateUserErrors, Unit> UpdateUser(TaggedUser user, StorageConnectionString connectionString)
        {
            return
                from blob in StorageFunctions.GetBlob(Container, GetIndexPath(user.Email), connectionString).ToAsync().Left(Cast.To<IUpdateUserErrors>())
                from json in SerializationFunctions.SerializeUser(user).ToAsync().Left(Cast.To<IUpdateUserErrors>())
                from _ in StorageFunctions.SetBlobText(blob, json, user.ETag).Left(Cast.To<IUpdateUserErrors>())
                select Prelude.unit;
        }

        public static EitherAsync<ILoadUserErrors, Option<TaggedUser>> LoadUser(Email email, StorageConnectionString connectionString)
        {
            return
                from blob in StorageFunctions.GetBlob(Container, GetIndexPath(email), connectionString).ToAsync().Left(Cast.To<ILoadUserErrors>())
                from maybeJson in StorageFunctions.GetBlobText(blob).Left(Cast.To<ILoadUserErrors>())
                from user in maybeJson
                    .Map(json => SerializationFunctions.DeserializeUser(json.Text, json.ETag))
                    .Sequence()
                    .ToAsync()
                    .Left(Cast.To<ILoadUserErrors>())
                select user;
        }
    }
}
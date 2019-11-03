using System;
using Web.Types.Errors;

namespace Web.Functions
{
    public static class MatchFunctions
    {
        public static T Match<T>(this IGetVaultErrors @this, Func<GeneralStorageError, T> generalStorage, Func<BearerTokenMissing, T> bearerTokenMissing, Func<MalformedPassword, T> malformedPassword, Func<MalformedETag, T> malformedETag, Func<MalformedBearerToken, T> malformedBearerToken, Func<UserIdMismatchError, T> userIdMismatch, Func<JsonDeserializationError, T> jsonDeserialization, Func<VaultIndexDoesNotExist, T> vaultIndexDoesNotExist, Func<PasswordIncorrectError, T> passwordIncorrect, Func<VaultDoesNotExistError, T> vaultDoesNotExist, Func<MalformedUserId, T> malformedUserId, Func<HashPasswordError, T> hashPassword, Func<MalformedEmail, T> malformedEmail, Func<EmailIncorrectError, T> emailIncorrect, Func<MalformedCloudStorageConnectionString, T> malformedCloudStorageConnectionString, Func<Base64DecodeError, T> base64Decode)
        {
            return @this switch {
                GeneralStorageError e => generalStorage(e),
                BearerTokenMissing e => bearerTokenMissing(e),
                MalformedPassword e => malformedPassword(e),
                MalformedETag e => malformedETag(e),
                MalformedBearerToken e => malformedBearerToken(e),
                UserIdMismatchError e => userIdMismatch(e),
                JsonDeserializationError e => jsonDeserialization(e),
                VaultIndexDoesNotExist e => vaultIndexDoesNotExist(e),
                PasswordIncorrectError e => passwordIncorrect(e),
                VaultDoesNotExistError e => vaultDoesNotExist(e),
                MalformedUserId e => malformedUserId(e),
                HashPasswordError e => hashPassword(e),
                MalformedEmail e => malformedEmail(e),
                EmailIncorrectError e => emailIncorrect(e),
                MalformedCloudStorageConnectionString e => malformedCloudStorageConnectionString(e),
                Base64DecodeError e => base64Decode(e),
                _ => throw new NotImplementedException("Missing match for " + @this.GetType().Name)
            };
        }
    }
}
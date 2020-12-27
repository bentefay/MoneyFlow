using System;
using Web.Types.Errors;

namespace Web.Functions
{
    public static class MatchFunctions
    {
        public static T Match<T>(this IGetVaultErrors @this, Func<GeneralStorageError, T> generalStorage, Func<BearerTokenMissing, T> bearerTokenMissing,
            Func<MalformedPassword, T> malformedPassword, Func<MalformedETag, T> malformedETag, Func<MalformedBearerToken, T> malformedBearerToken,
            Func<UserIdMismatchError, T> userIdMismatch, Func<JsonDeserializationError, T> jsonDeserialization, Func<UserDoesNotExistError, T> userDoesNotExist,
            Func<PasswordIncorrectError, T> passwordIncorrect, Func<VaultDoesNotExistError, T> vaultDoesNotExist, Func<MalformedUserId, T> malformedUserId,
            Func<HashPasswordError, T> hashPassword, Func<MalformedEmail, T> malformedEmail, Func<EmailIncorrectError, T> emailIncorrect,
            Func<MalformedCloudStorageConnectionString, T> malformedCloudStorageConnectionString, Func<Base64DecodeError, T> base64Decode)
        {
            return @this switch
            {
                GeneralStorageError e => generalStorage(e),
                BearerTokenMissing e => bearerTokenMissing(e),
                MalformedPassword e => malformedPassword(e),
                MalformedETag e => malformedETag(e),
                MalformedBearerToken e => malformedBearerToken(e),
                UserIdMismatchError e => userIdMismatch(e),
                JsonDeserializationError e => jsonDeserialization(e),
                UserDoesNotExistError e => userDoesNotExist(e),
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

        public static T Match<T>(this ICreateUserErrors @this, Func<JsonDeserializationError, T> jsonDeserialization,
            Func<CouldNotUpdateBlobBecauseTheETagHasChanged, T> couldNotUpdateBlobBecauseTheETagHasChanged,
            Func<CouldNotCreateBlobBecauseItAlreadyExistsError, T> couldNotCreateBlobBecauseItAlreadyExists, Func<MalformedPassword, T> malformedPassword,
            Func<GeneratePasswordSaltError, T> generateSalt, Func<MalformedCloudStorageConnectionString, T> malformedCloudStorageConnectionString, Func<MalformedETag, T> malformedETag,
            Func<Base64DecodeError, T> base64Decode, Func<GeneralStorageError, T> generalStorage, Func<MalformedEmail, T> malformedEmail, Func<HashPasswordError, T> hashPassword,
            Func<JsonSerializationError, T> jsonSerialization, Func<BearerTokenMissing, T> bearerTokenMissing, Func<MalformedBearerToken, T> malformedBearerToken)
        {
            return @this switch
            {
                JsonDeserializationError e => jsonDeserialization(e),
                CouldNotUpdateBlobBecauseTheETagHasChanged e => couldNotUpdateBlobBecauseTheETagHasChanged(e),
                CouldNotCreateBlobBecauseItAlreadyExistsError e => couldNotCreateBlobBecauseItAlreadyExists(e),
                MalformedPassword e => malformedPassword(e),
                GeneratePasswordSaltError e => generateSalt(e),
                MalformedCloudStorageConnectionString e => malformedCloudStorageConnectionString(e),
                MalformedETag e => malformedETag(e),
                Base64DecodeError e => base64Decode(e),
                GeneralStorageError e => generalStorage(e),
                MalformedEmail e => malformedEmail(e),
                HashPasswordError e => hashPassword(e),
                JsonSerializationError e => jsonSerialization(e),
                BearerTokenMissing e => bearerTokenMissing(e),
                MalformedBearerToken e => malformedBearerToken(e),
                _ => throw new NotImplementedException("Missing match for " + @this.GetType().Name)
            };
        }

        public static T Match<T>(this IUpdateVaultErrors @this, Func<CouldNotCreateBlobBecauseItAlreadyExistsError, T> couldNotCreateBlobBecauseItAlreadyExists,
            Func<MalformedEmail, T> malformedEmail, Func<EmailIncorrectError, T> emailIncorrect, Func<JsonSerializationError, T> jsonSerialization,
            Func<MalformedPassword, T> malformedPassword, Func<MalformedETag, T> malformedETag, Func<HashPasswordError, T> hashPassword,
            Func<PasswordIncorrectError, T> passwordIncorrect, Func<UserDoesNotExistError, T> userDoesNotExist, Func<MalformedUserId, T> malformedUserId,
            Func<VaultDoesNotExistError, T> vaultDoesNotExist, Func<JsonDeserializationError, T> jsonDeserialization,
            Func<MalformedCloudStorageConnectionString, T> malformedCloudStorageConnectionString, Func<GeneralStorageError, T> generalStorage,
            Func<BearerTokenMissing, T> bearerTokenMissing, Func<Base64DecodeError, T> base64Decode,
            Func<CouldNotUpdateBlobBecauseTheETagHasChanged, T> couldNotUpdateBlobBecauseTheETagHasChanged, Func<UserIdMismatchError, T> userIdMismatch,
            Func<MalformedBearerToken, T> malformedBearerToken)
        {
            return @this switch
            {
                CouldNotCreateBlobBecauseItAlreadyExistsError e => couldNotCreateBlobBecauseItAlreadyExists(e),
                MalformedEmail e => malformedEmail(e),
                EmailIncorrectError e => emailIncorrect(e),
                JsonSerializationError e => jsonSerialization(e),
                MalformedPassword e => malformedPassword(e),
                MalformedETag e => malformedETag(e),
                HashPasswordError e => hashPassword(e),
                PasswordIncorrectError e => passwordIncorrect(e),
                UserDoesNotExistError e => userDoesNotExist(e),
                MalformedUserId e => malformedUserId(e),
                VaultDoesNotExistError e => vaultDoesNotExist(e),
                JsonDeserializationError e => jsonDeserialization(e),
                MalformedCloudStorageConnectionString e => malformedCloudStorageConnectionString(e),
                GeneralStorageError e => generalStorage(e),
                BearerTokenMissing e => bearerTokenMissing(e),
                Base64DecodeError e => base64Decode(e),
                CouldNotUpdateBlobBecauseTheETagHasChanged e => couldNotUpdateBlobBecauseTheETagHasChanged(e),
                UserIdMismatchError e => userIdMismatch(e),
                MalformedBearerToken e => malformedBearerToken(e),
                _ => throw new NotImplementedException("Missing match for " + @this.GetType().Name)
            };
        }
    }
}
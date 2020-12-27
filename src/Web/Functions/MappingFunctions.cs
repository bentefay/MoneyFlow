using LanguageExt;
using Web.Types;
using Web.Types.Dtos.Storage;
using Web.Types.Dtos.Web;
using Web.Types.Errors;
using Web.Types.Values;
using Web.Utils.Extensions;

namespace Web.Functions
{
    public static class MappingFunctions
    {
        public static UserDto ToDto(User user)
        {
            return new UserDto(
                UserId: user.UserId.Value.ToString(),
                Email: user.Email.Value,
                PasswordSalt: user.PasswordSalt.Value,
                Password:  user.Password.Value);
        }

        public static Either<IDeserializeUserErrors, TaggedUser> FromDto(UserDto dto, StorageETag eTag)
        {
            return
                from userId in UserId.Create(dto.UserId).Left(Cast.To<IDeserializeUserErrors>())
                from email in Email.Create(dto.Email).Left(Cast.To<IDeserializeUserErrors>())
                let salt = new PasswordSalt(dto.PasswordSalt)
                from password in DoubleHashedPassword.Create(dto.Password).Left(Cast.To<IDeserializeUserErrors>())
                select new TaggedUser(userId, email, salt, password, eTag);
        }

        public static GetVaultResponse ToDto(TaggedVault vault)
        {
            return new GetVaultResponse(
                UserId: vault.UserId.Value.ToString(),
                Content: vault.Content.Value,
                ETag: vault.ETag.Value);
        }

        public static CreateUserResponse ToDto(UserId userId)
        {
            return new CreateUserResponse(UserId: userId.Value.ToString());
        }

        public static Either<IUpdateVaultRequestToVaultErrors, Vault> FromDto(UpdateVaultRequest request)
        {
            return 
                from userId in UserId.Create(request.UserId).Left(Cast.To<IUpdateVaultRequestToVaultErrors>())
                let content = new Base64EncodedEncryptedVault(request.Content)
                from maybeEtag in Prelude.Optional(request.ETag!)
                    .Map(StorageETag.Create)
                    .Sequence()
                    .Left(Cast.To<IUpdateVaultRequestToVaultErrors>())
                select maybeEtag.Match(
                    Some: eTag => new TaggedVault(userId, content, eTag),
                    None: () => new Vault(userId, content));
        }
    }
}
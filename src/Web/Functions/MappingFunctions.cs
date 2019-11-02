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
        public static VaultIndexDto ToDto(VaultIndex vaultIndex)
        {
            return new VaultIndexDto(
                userId: vaultIndex.UserId.Value.ToString(),
                email: vaultIndex.Email.Value,
                passwordSalt: vaultIndex.PasswordSalt.Value,
                password:  vaultIndex.Password.Value);
        }

        public static Either<IDeserializeVaultIndexErrors, TaggedVaultIndex> FromDto(VaultIndexDto dto, StorageETag eTag)
        {
            return
                from userId in UserId.Create(dto.UserId).Left(Cast.To<IDeserializeVaultIndexErrors>())
                from email in Email.Create(dto.Email).Left(Cast.To<IDeserializeVaultIndexErrors>())
                let salt = new PasswordSalt(dto.PasswordSalt)
                from password in DoubleHashedPassword.Create(dto.Password).Left(Cast.To<IDeserializeVaultIndexErrors>())
                select new TaggedVaultIndex(userId, email, salt, password, eTag);
        }

        public static GetVaultResponse ToDto(TaggedVault vault)
        {
            return new GetVaultResponse(
                userId: vault.UserId.Value.ToString(),
                content: vault.Content.Value,
                eTag: vault.ETag.Value);
        }

        public static CreateVaultResponse ToDto(UserId userId)
        {
            return new CreateVaultResponse(userId: userId.Value.ToString());
        }

        public static Either<IUpdateVaultRequestToVaultErrors, Vault> FromDto(UpdateVaultRequest request)
        {
            return 
                from userId in UserId.Create(request.UserId).Left(Cast.To<IUpdateVaultRequestToVaultErrors>())
                let content = new Base64EncodedEncryptedVault(request.Content)
                from maybeEtag in Prelude.Optional(request.ETag).Map(StorageETag.Create).Sequence().Left(Cast.To<IUpdateVaultRequestToVaultErrors>())
                select maybeEtag.Match(
                    Some: eTag => new TaggedVault(userId, content, eTag),
                    None: () => new Vault(userId, content));
        }
    }
}
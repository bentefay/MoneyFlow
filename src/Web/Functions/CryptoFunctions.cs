using System;
using System.Security.Cryptography;
using System.Xml.Serialization;
using LanguageExt;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Web.Types;
using Web.Types.Errors;
using Web.Types.Values;

namespace Web.Functions
{
    public static class CryptoFunctions
    {
        private const int SaltLengthInBytes = 128 / 8;

        public static Either<GeneratePasswordSaltError, PasswordSalt> GeneratePasswordSalt()
        {
            try
            {
                var salt = new byte[SaltLengthInBytes];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }

                return new PasswordSalt(Convert.ToBase64String(salt));
            }
            catch (Exception e)
            {
                return new GeneratePasswordSaltError(e);
            }
        }

        public static Either<HashPasswordError, DoubleHashedPassword> HashPassword(HashedPassword password, PasswordSalt salt)
        {
            try
            {
                var saltBytes = Convert.FromBase64String(salt.Value);

                var hashedPasswordBytes = KeyDerivation.Pbkdf2(
                    password: password.Value,
                    salt: saltBytes,
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: SaltLengthInBytes);

                var hashedPasswordChars = Convert.ToBase64String(hashedPasswordBytes);

                return new DoubleHashedPassword(hashedPasswordChars);
            }
            catch (Exception e)
            {
                return new HashPasswordError(e);
            }
        }
    }
}
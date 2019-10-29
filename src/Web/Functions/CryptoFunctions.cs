using System;
using System.Security.Cryptography;
using System.Xml.Serialization;
using LanguageExt;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Web.Types;
using Web.Types.Errors;

namespace Web.Functions
{
    public class CryptoFunctions
    {
        private static int _bytes = 128 / 8;

        public static Either<GenerateSaltError, PasswordSalt> CreatePasswordSalt()
        {
            try
            {
                var salt = new byte[_bytes];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }

                return new PasswordSalt(Convert.ToBase64String(salt));
            }
            catch (Exception e)
            {
                return new GenerateSaltError(e);
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
                    numBytesRequested: _bytes);

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
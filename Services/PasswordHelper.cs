using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace EquidCMS.Services
{

    public class PasswordHelper
    {
        public string HashPassword(string password)
        {
            // Generate a salt (16 bytes is recommended)
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Derive a 256-bit subkey (use PBKDF2 with HMACSHA256)
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,  // 10,000 iterations is recommended
                numBytesRequested: 256 / 8
            ));

            // Combine the salt and the hashed password
            return Convert.ToBase64String(salt) + "$" + hashedPassword;
        }

        public bool VerifyPassword(string enteredPassword, string storedHash)
        {
            var parts = storedHash.Split('$');
            byte[] salt = Convert.FromBase64String(parts[0]);
            string storedPasswordHash = parts[1];

            // Hash the entered password with the stored salt
            string enteredPasswordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: enteredPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));

            // Check if the entered password hash matches the stored one
            return storedPasswordHash == enteredPasswordHash;
        }
    }
}

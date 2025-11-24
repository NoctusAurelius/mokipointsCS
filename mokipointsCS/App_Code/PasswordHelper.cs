using System;
using System.Security.Cryptography;
using System.Text;

namespace mokipointsCS
{
    /// <summary>
    /// Helper class for password hashing and verification
    /// </summary>
    public class PasswordHelper
    {
        /// <summary>
        /// Hashes a password using SHA256
        /// </summary>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be null or empty", "password");
            }

            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Compute hash
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Verifies a password against a hash
        /// </summary>
        public static bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
            {
                return false;
            }

            string passwordHash = HashPassword(password);
            return passwordHash.Equals(hash, StringComparison.OrdinalIgnoreCase);
        }
    }
}


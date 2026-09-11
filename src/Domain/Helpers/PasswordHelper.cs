using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Domain.Helpers
{
    public static class PasswordHelper
    {

        public static string getPassword(string Password)
        {
            var hash = GetHash(Password);
            if (hash == null || hash.Length == 0)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        /// <summary>
        /// SHA-256 of the password. Null/empty input is treated as no hash
        /// (invalid credentials) instead of throwing ArgumentNullException.
        /// </summary>
        public static byte[] GetHash(string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
            {
                return Array.Empty<byte>();
            }

            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

    }
}

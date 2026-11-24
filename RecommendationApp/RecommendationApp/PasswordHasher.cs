using System;
using System.Security.Cryptography;

namespace RecommendationApp
{
    /// <summary>
    /// Password hashing with PBKDF2-HMAC-SHA256 (Rfc2898DeriveBytes).
    ///
    /// Plain-text passwords are never stored. Each password gets its own random
    /// salt, and the iteration count makes brute-forcing a stolen hash expensive.
    ///
    /// Stored format: pbkdf2-sha256$iterations$salt$hash  (salt and hash in base64)
    ///
    /// The algorithm name and iteration count are part of the string, so the cost
    /// can be raised later without invalidating hashes already in the database.
    /// </summary>
    public static class PasswordHasher
    {
        private const string Algorithm = "pbkdf2-sha256";
        private const int SaltSize = 16;       // 128-bit salt
        private const int KeySize = 32;        // 256-bit derived key
        private const int Iterations = 100000;

        public static string Hash(string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] key;
            // SHA-1 is the default of the shorter overloads and is not acceptable
            // here; the algorithm is named explicitly.
            using (var derive = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                key = derive.GetBytes(KeySize);
            }

            return string.Format("{0}${1}${2}${3}",
                Algorithm,
                Iterations,
                Convert.ToBase64String(salt),
                Convert.ToBase64String(key));
        }

        public static bool Verify(string password, string stored)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(stored))
                return false;

            string[] parts = stored.Split('$');
            if (parts.Length != 4 || parts[0] != Algorithm)
                return false;

            int iterations;
            if (!int.TryParse(parts[1], out iterations) || iterations <= 0)
                return false;

            byte[] salt, expected;
            try
            {
                salt = Convert.FromBase64String(parts[2]);
                expected = Convert.FromBase64String(parts[3]);
            }
            catch (FormatException)
            {
                return false;
            }

            byte[] actual;
            using (var derive = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                actual = derive.GetBytes(expected.Length);
            }

            return FixedTimeEquals(actual, expected);
        }

        /// <summary>
        /// Compares two byte arrays without leaking, through timing, how many
        /// leading bytes matched. A plain == loop would.
        /// </summary>
        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }

            return diff == 0;
        }
    }
}

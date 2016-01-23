namespace s2.s2Utils.Logic.Base.Utilities
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Cryptography;
    using System.Security.Permissions;
    using System.Text;

    using s2.s2Utils.Logic.Portable.Utilities;

    /// <summary>
    /// Provides logic wrappers for simple calls to the namespace <see cref="System.Security"/>.
    /// </summary>
    /// <remarks>
    /// Basic idea and source taken from http://www.obviex.com/samples/hash.aspx.
    /// </remarks>
    public static class SecurityUtil
    {
        #region constants

        /// <summary>
        /// Defines the maximum size for hash-salts in bytes.
        /// </summary>
        private const int MaxSaltSize = 16;

        /// <summary>
        /// Defines the minimum size for hash-salts in bytes.
        /// </summary>
        private const int MinSaltSize = 4;

        #region static fields

        /// <summary>
        /// Defines valid keys for hash algorithms.
        /// </summary>
        private static readonly string[] ValidHashAlgorithms = { "SHA1", "SHA256", "SHA384", "SHA512", "HMAC256", "HMAC384", "HMAC512" };

        #endregion

        #endregion

        #region methods

        /// <summary>
        /// Generates a hash for the given plain text value and returns a
        /// base64-encoded result. Before the hash is computed, a random salt
        /// is generated and appended to the plain text. This salt is stored at
        /// the end of the hash value, so it can be used later for hash
        /// verification.
        /// </summary>
        /// <param name="plainText">Plaintext value to be hashed. The function does not check whether this parameter is null.</param>
        /// <param name="hashAlgorithm">Case-insensitve name of the hash algorithm. Allowed values are: "SHA1", "SHA256", "SHA384", "SHA512", "HMAC256", "HMAC384", "HMAC512"</param>
        /// <param name="salt">Salt bytes. This parameter can be null, in which case a random salt value will be generated.</param>
        /// <param name="urlEncode">Indicates whether the result should be URL encoded.</param>
        /// <returns>Hash value formatted as a base64-encoded string.</returns>
        public static string ComputeHash(string plainText, string hashAlgorithm, byte[] salt, bool urlEncode = true)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => plainText);
            if (string.IsNullOrEmpty(hashAlgorithm) || !ValidHashAlgorithms.Contains(hashAlgorithm.ToUpper(CultureInfo.InvariantCulture)))
            {
                throw new ArgumentException("Provided algorithm is unknown.", nameof(hashAlgorithm));
            }
            // If salt is not specified, generate it on the fly.
            if (salt == null)
            {
                // Generate a random number for the size of the salt.
                var randomizer = new Random();
                var saltSize = randomizer.Next(MinSaltSize, MaxSaltSize);
                // Allocate a byte array, which will hold the salt.
                salt = new byte[saltSize];
                // Initialize a random number generator.
                var rng = new RNGCryptoServiceProvider();
                // Fill the salt with cryptographically strong byte values.
                rng.GetNonZeroBytes(salt);
            }
            else if (salt.Length < MinSaltSize || salt.Length > MaxSaltSize)
            {
                throw new ArgumentException("Salt length invalid. Must be between 4 and 16", nameof(salt));
            }
            var hash = GetAlgorithm(hashAlgorithm);
            if (hash == null)
            {
                throw new InvalidOperationException("Could not obtain hasher.");
            }
            // Convert plain text into a byte array.
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            // Allocate array, which will hold plain text and salt.
            var plainTextWithSaltBytes = new byte[plainTextBytes.Length + salt.Length];
            // Copy plain text bytes into resulting array.
            for (var i = 0; i < plainTextBytes.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainTextBytes[i];
            }
            // Append salt bytes to the resulting array.
            for (var i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainTextBytes.Length + i] = salt[i];
            }
            // Compute hash value of our plain text with appended salt.
            var hashBytes = hash.ComputeHash(plainTextWithSaltBytes);
            // Create array which will hold hash and original salt bytes.
            var hashWithSaltBytes = new byte[hashBytes.Length + salt.Length];
            // Copy hash bytes into resulting array.
            for (var i = 0; i < hashBytes.Length; i++)
            {
                hashWithSaltBytes[i] = hashBytes[i];
            }
            // Append salt bytes to the result.
            for (var i = 0; i < salt.Length; i++)
            {
                hashWithSaltBytes[hashBytes.Length + i] = salt[i];
            }
            // Convert result into a base64-encoded string.
            var hashValue = Convert.ToBase64String(hashWithSaltBytes);
            if (urlEncode)
            {
                // Caller wants to url encode the result
                hashValue = WebUtility.UrlEncode(hashValue);
            }
            hash.Dispose();
            // Return the result.
            return hashValue;
        }

        /// <summary>
        /// Decrypts a given <paramref name="cipherText"/> symetrically using <see cref="RijndaelManaged"/>.
        /// </summary>
        /// <param name="cipherText">The encrypted Base64-encoded text.</param>
        /// <param name="key">The secret key.</param>
        /// <param name="iv">The initialization vector of the alogrithm.</param>
        /// <returns>The decrypted text.</returns>
        public static string Decrypt(this string cipherText, byte[] key, byte[] iv)
        {
            return DecryptText(cipherText, key, iv);
        }

        /// <summary>
        /// Decrypts a given <paramref name="cipherText"/> symetrically using <see cref="RijndaelManaged"/>.
        /// </summary>
        /// <param name="cipherText">The encrypted Base64-encoded text.</param>
        /// <param name="key">The secret key.</param>
        /// <param name="iv">The initialization vector of the alogrithm.</param>
        /// <returns>The decrypted text.</returns>
        public static string DecryptText(string cipherText, byte[] key, byte[] iv)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => cipherText);
            CheckUtil.ThrowIfNull(() => key);
            CheckUtil.ThrowIfNull(() => iv);
            string plaintext;
            using (var rijndael = new RijndaelManaged())
            {
                rijndael.Key = key;
                rijndael.IV = iv;
                var decryptor = rijndael.CreateDecryptor(rijndael.Key, rijndael.IV);
                var cipherTextBytes = Convert.FromBase64String(cipherText);
                var memStream = new MemoryStream(cipherTextBytes);
                var cryptStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read);
                using (var reader = new StreamReader(cryptStream))
                {
                    plaintext = reader.ReadToEnd();
                }
            }
            return plaintext;
        }

        /// <summary>
        /// Encrypts a given <paramref name="plainText"/> symetrically using <see cref="RijndaelManaged"/>.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <param name="key">The secret key.</param>
        /// <param name="iv">The initialization vector of the alogrithm.</param>
        /// <returns>The encrypted text coded in Base64.</returns>
        public static string Encrypt(this string plainText, byte[] key, byte[] iv)
        {
            return EncryptText(plainText, key, iv);
        }

        /// <summary>
        /// Encrypts a given <paramref name="plainText"/> symetrically using <see cref="RijndaelManaged"/>.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <param name="key">The secret key.</param>
        /// <param name="iv">The initialization vector of the alogrithm.</param>
        /// <returns>The encrypted text coded in Base64.</returns>
        public static string EncryptText(string plainText, byte[] key, byte[] iv)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => plainText);
            CheckUtil.ThrowIfNull(() => key);
            CheckUtil.ThrowIfNull(() => iv);
            string encrypted;
            using (var rijndael = new RijndaelManaged())
            {
                rijndael.Key = key;
                rijndael.IV = iv;
                var encryptor = rijndael.CreateEncryptor(rijndael.Key, rijndael.IV);
                var memStream = new MemoryStream();
                var cryptStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write);
                using (var writer = new StreamWriter(cryptStream))
                {
                    writer.Write(plainText);
                }
                encrypted = Convert.ToBase64String(memStream.ToArray());
            }
            return encrypted;
        }

        /// <summary>
        /// Retrieves the byte-array corresponding to the provided <paramref name="text"/> using provided <paramref name="encoding"/>.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="encoding">The encoding or <c>null</c> if <see cref="Encoding.Default"/> should be used.</param>
        /// <returns>The bytes of the <paramref name="text"/>.</returns>
        public static byte[] GetBytes(this string text, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.Default;
            }
            return encoding.GetBytes(text);
        }

        /// <summary>
        /// Generates a random text of a specified <paramref name="desiredLength"/>.
        /// </summary>
        /// <param name="desiredLength">The resulting length of the text.</param>
        /// <returns>The generated text converted to Base64.</returns>
        public static string GetRandomText(int desiredLength)
        {
            if (desiredLength < 1)
            {
                throw new ArgumentException("Desired lenght invalid.", "desiredLength");
            }
            // Allocate a byte array, which will hold the salt.
            var bytes = new byte[desiredLength];
            // Initialize a random number generator.
            using (var rng = new RNGCryptoServiceProvider())
            {
                // Fill the salt with cryptographically strong byte values.
                rng.GetNonZeroBytes(bytes);
            }
            // Encode and return
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Checks if the current user has the <paramref name="permission"/> on the <paramref name="filePath"/>.
        /// </summary>
        /// <remarks>
        /// Uses <see cref="FileIOPermission"/> internally.
        /// </remarks>
        /// <param name="filePath">The absolute path to the file.</param>
        /// <param name="permission">The permission to check.</param>
        /// <returns><c>true</c> if the current user has the desired <paramref name="permission"/>, otherwise <c>false</c>.</returns>
        public static bool HasUserAccessToFile(string filePath, FileIOPermissionAccess permission = FileIOPermissionAccess.Read)
        {
            var result = false;
            try
            {
                var perm = new FileIOPermission(permission, filePath);
                perm.Demand();
                result = true;
            }
            catch (Exception)
            {
            }
            return result;
        }

        /// <summary>
        /// Generates a secure string out of a given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The string to transform.</param>
        /// <returns>The secure version of the <paramref name="value"/>.</returns>
        public static SecureString ToSecureString(this string value)
        {
            var result = new SecureString();
            value.ToList().ForEach(result.AppendChar);
            result.MakeReadOnly();
            return result;
        }

        /// <summary>
        /// Gets the value for a given <paramref name="secureValue"/>.
        /// </summary>
        /// <param name="secureValue">The secured text.</param>
        /// <returns>The readable text.</returns>
        public static string ToUnsecureString(this SecureString secureValue)
        {
            CheckUtil.ThrowIfNull(() => secureValue);
            var unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureValue);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        /// <summary>
        /// Compares a hash of the specified plain text value to a given hash
        /// value. Plain text is hashed with the same salt value as the original
        /// hash.
        /// </summary>
        /// <param name="plainText">Plain text to be verified against the specified hash.</param>
        /// <param name="hashAlgorithm">Case-insensitve name of the hash algorithm. Allowed values are: "SHA1", "SHA256", "SHA384", "SHA512", "HMAC256", "HMAC384", "HMAC512"</param>
        /// <param name="hashValue">Base64-encoded hash value produced by ComputeHash function. This value includes the original salt appended to it.</param>
        /// <param name="urlDecode">Indicates whether the result should be URL encoded.</param>
        /// <returns><c>true</c> if the <paramref name="plainText"/> will be hashed to <paramref name="hashValue"/>, otherwise <c>false</c>.</returns>
        public static bool VerifyHash(string plainText, string hashAlgorithm, string hashValue, bool urlDecode = true)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => plainText);
            CheckUtil.ThrowIfNullOrEmpty(() => hashValue);
            CheckUtil.ThrowIfNullOrEmpty(() => hashAlgorithm);
            if (!ValidHashAlgorithms.Contains(hashAlgorithm.ToUpper(CultureInfo.InvariantCulture)))
            {
                throw new ArgumentException("Provided algorithm is unknown.", "hashAlgorithm");
            }
            var hash = GetAlgorithm(hashAlgorithm);
            if (hash == null)
            {
                throw new InvalidOperationException("Could not obtain hasher.");
            }
            // If the caller flagged this call with urlDecode we have to UrlDecode the hashValue. If not,
            // the decoded value will be the passed one. This variable is important, because at the end
            // we have to compare the given hashValue and not the decoded one.
            var decodedHashValue = hashValue;
            if (urlDecode)
            {
                decodedHashValue = WebUtility.UrlDecode(hashValue);
            }
            // Convert base64-encoded hash value into a byte array.
            var hashWithSaltBytes = Convert.FromBase64String(decodedHashValue);
            // We must know size of hash (without salt).
            var hashSizeInBytes = hash.HashSize / 8;
            // Make sure that the specified hash value is long enough.
            if (hashWithSaltBytes.Length < hashSizeInBytes)
            {
                hash.Dispose();
                return false;
            }
            // Allocate array to hold original salt bytes retrieved from hash.
            var saltBytes = new byte[hashWithSaltBytes.Length - hashSizeInBytes];
            // Copy salt from the end of the hash to the new array.
            for (var i = 0; i < saltBytes.Length; i++)
            {
                saltBytes[i] = hashWithSaltBytes[hashSizeInBytes + i];
            }
            // Compute a new hash string.
            var expectedHashString = ComputeHash(plainText, hashAlgorithm, saltBytes, urlDecode);
            hash.Dispose();
            // If the computed hash matches the specified hash, the plain text value must be correct.
            return (hashValue.Equals(expectedHashString, StringComparison.Ordinal));
        }

        /// <summary>
        /// Tries to retrieve the hash alsgorithm for a given <paramref name="algorithmKey"/>.
        /// </summary>
        /// <param name="algorithmKey">The key of the algorithm which must be a entry in <see cref="ValidHashAlgorithms"/>.</param>
        /// <returns>The algorithm or <c>null</c> if no was found.</returns>
        private static HashAlgorithm GetAlgorithm(string algorithmKey)
        {
            switch (algorithmKey.ToUpper(CultureInfo.InvariantCulture))
            {
                case "SHA1":
                    return new SHA1Managed();
                case "SHA256":
                    return new SHA256Managed();
                case "SHA384":
                    return new SHA384Managed();
                case "SHA512":
                    return new SHA512Managed();
                case "HMAC256":
                    return new HMACSHA256();
                case "HMAC384":
                    return new HMACSHA384();
                case "HMAC512":
                    return new HMACSHA512();
            }
            return null;
        }

        #endregion
    }
}
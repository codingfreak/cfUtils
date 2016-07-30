using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codingfreaks.cfUtils.Logic.WebMvcUtils.Utils
{
    using System.Security.Cryptography;

    using Microsoft.AspNet.Identity;

    /// <summary>
    /// Implementation of the default pass hasher taken from
    /// https://github.com/aspnet/Identity/blob/a8ba99bc5b11c5c48fc31b9b0532c0d6791efdc8/src/Microsoft.AspNetCore.Identity/PasswordHasher.cs.
    /// </summary>
    public class CustomPasswordHasher : IPasswordHasher
    {
        #region explicit interfaces

        /// <summary>
        /// Hash a password
        /// </summary>
        /// <param name="password"></param>
        /// <returns>The hashed password as a Base64 string.</returns>
        public string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }
            using (var bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            var dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return Convert.ToBase64String(dst);
        }

        /// <summary>
        /// Verifies that a password matches the hashed password
        /// </summary>
        /// <param name="hashedPassword">The hashed password.</param>
        /// <param name="password">The given clear password.</param>
        /// <returns></returns>
        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string password)
        {
            byte[] buffer4;
            if (hashedPassword == null)
            {
                return PasswordVerificationResult.Failed;
            }
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }
            var src = Convert.FromBase64String(hashedPassword);
            if ((src.Length != 0x31) || (src[0] != 0))
            {
                return PasswordVerificationResult.Failed;
            }
            var dst = new byte[0x10];
            Buffer.BlockCopy(src, 1, dst, 0, 0x10);
            var buffer3 = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
            using (var bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
            {
                buffer4 = bytes.GetBytes(0x20);
            }
            var ok = ByteArraysEqual(buffer3, buffer4);
            return ok ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }

        #endregion

        #region methods

        /// <summary>
        /// Compares two byte arrays for equality. The method is specifically written so that the loop is not optimized.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (a == null && b == null)
            {
                return true;
            }
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }
            var areSame = true;
            for (var i = 0; i < a.Length; i++)
            {
                areSame &= a[i] == b[i];
            }
            return areSame;
        }

        #endregion
    }
}

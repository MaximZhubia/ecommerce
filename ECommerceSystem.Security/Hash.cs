using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ECommerceSystem.Security
{
    public static class Hash
    {
        public static string SHA512TripleHash(string input)
        {
            using SHA512Managed sha512 = new SHA512Managed();
            byte[] data = Encoding.ASCII.GetBytes(input);
            byte[] hash = sha512.ComputeHash(data);
            hash = sha512.ComputeHash(hash);
            hash = sha512.ComputeHash(hash);
            string hashString = BitConverter.ToString(hash);

            return hashString.Substring(0, hashString.Length / 2);
        }

        public static uint GeneratePvvCode()
        {
            var cryptoRng = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[4];

            cryptoRng.GetBytes(buffer);
            var value = BitConverter.ToUInt32(buffer, 0);

            return value % 8999 + 1000;
        }

        public static string GetPvvHash(uint pvv)
        {
            using var sha512 = new SHA512Managed();

            var data = BitConverter.GetBytes(pvv);
            var hash = sha512.ComputeHash(data);
            hash = sha512.ComputeHash(hash);
            hash = sha512.ComputeHash(hash);
            var hashString = BitConverter.ToString(hash);

            return hashString.Substring(0, hash.Length / 2);
        }
    }
}

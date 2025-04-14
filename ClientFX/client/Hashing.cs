using System;
using System.Security.Cryptography;
using System.Text;

namespace Lab8Csharp.client
{
    public static class Hashing
    {
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder hexString = new StringBuilder();

                foreach (byte b in hashBytes)
                {
                    hexString.AppendFormat("{0:x2}", b);
                }

                return hexString.ToString();
            }
        }
    }
}
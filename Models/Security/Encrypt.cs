using System.Security.Cryptography;
using System.Text;

namespace MyFinances.WebApi.Models.Security
{
    public class Encrypt
    {
        public static string GetEncryptedPassword(string password)
        {
            SHA256 sHA256 = SHA256.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            StringBuilder encryptedPasswordBuilder = new StringBuilder();
            byte[] bytes = sHA256.ComputeHash(encoding.GetBytes(password));

            for (int i = 0; i < bytes.Length; i++)
                encryptedPasswordBuilder.AppendFormat("{0:x1}", bytes[i]);
            return encryptedPasswordBuilder.ToString();
        }
    }
}

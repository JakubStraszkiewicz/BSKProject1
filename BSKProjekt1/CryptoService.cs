using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Org.BouncyCastle;

namespace BSKProject1
{
    class CryptoService
    {
        public string createSha1Hash(string message,int length)
        {
            
            byte[] encrypted;
            string hash;

            using (SHA512CryptoServiceProvider sha1 = new SHA512CryptoServiceProvider())
            {
                
                encrypted = sha1.ComputeHash(ASCIIEncoding.ASCII.GetBytes(message), 0, message.Length);
                hash = Convert.ToBase64String(encrypted);
            }

            while(hash.Length < length)
                hash += hash;

            if (hash.Length > length)
                hash = hash.Substring(0, length);

            return hash;
        }

        public string aesEncoding(string key, String mode, int blockSize, string message, byte[] iv)
        {
            byte[] encrypted;

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                byte[] a = ASCIIEncoding.ASCII.GetBytes(key);
                aes.KeySize = a.Length*8;
                aes.Key = ASCIIEncoding.ASCII.GetBytes(key);
                aes.BlockSize = blockSize;
                aes.IV = iv;

                switch (mode)
                {
                    case "ECB":
                        aes.Mode = CipherMode.ECB;
                        break;
                    case "CBC":
                        aes.Mode = CipherMode.CBC;
                        break;
                }
                aes.Padding = PaddingMode.PKCS7;
                ICryptoTransform transform = aes.CreateEncryptor();
                encrypted = transform.TransformFinalBlock(ASCIIEncoding.ASCII.GetBytes(message), 0, message.Length);
            }
            return Convert.ToBase64String(encrypted);
        }

        public string aesDecoding(string key, String mode, int blockSize, string message, byte[] iv)
        {
            byte[] decrypted;

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                byte[] byteKey = ASCIIEncoding.ASCII.GetBytes(key);
                aes.KeySize = byteKey.Length * 8;
                aes.Key = ASCIIEncoding.ASCII.GetBytes(key);
                aes.BlockSize = blockSize;
                aes.IV = iv;
               
                switch (mode)
                {
                    case "ECB":
                        aes.Mode = CipherMode.ECB;
                        break;
                    case "CBC":
                        aes.Mode = CipherMode.CBC;
                        break;
                }
                aes.Padding = PaddingMode.PKCS7;
                ICryptoTransform transform = aes.CreateDecryptor();
                byte[] encryptedMessage = Convert.FromBase64String(message);
                decrypted = transform.TransformFinalBlock(encryptedMessage, 0, encryptedMessage.Length);
                
            }
            return ASCIIEncoding.ASCII.GetString(decrypted);
        }
    }
}

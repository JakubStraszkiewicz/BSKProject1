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
        public byte[] createSha512Hash(string message,int length)
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

            return Convert.FromBase64String(hash);
        }

        public byte[] aesEncoding(byte[] key, String mode, int blockSize, byte[] message, byte[] iv)
        {
            byte[] encrypted;

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.KeySize = key.Length*8;
                aes.Key = key;
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
                encrypted = transform.TransformFinalBlock(message, 0, message.Length);
            }
            return encrypted;
        }


        public byte[] aesDecoding(byte[] key, String mode, int blockSize, byte[] message, byte[] iv)
        {
            byte[] decrypted;

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.KeySize = key.Length * 8;
                aes.Key = key;
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
                byte[] encryptedMessage = message;
                decrypted = transform.TransformFinalBlock(encryptedMessage, 0, encryptedMessage.Length);
                
            }
            return decrypted;
        }
    }
}

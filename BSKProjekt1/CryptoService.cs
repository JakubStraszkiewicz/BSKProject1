using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Crypto = Org.BouncyCastle.Crypto;

namespace BSKProject1
{
    class CryptoService
    {
        public byte[] createSha512Hash(string message,int length)
        {
            int encryptedLength = 0;
            byte[] hash;
            byte[] encrypted = new byte[length];

            using (SHA512CryptoServiceProvider sha1 = new SHA512CryptoServiceProvider())
            {
                
                hash = sha1.ComputeHash(ASCIIEncoding.ASCII.GetBytes(message), 0, message.Length);
            }

            Array.Copy(hash, encrypted, length);

            return encrypted;
        }

        public byte[] ofbEncoding(byte[] key, byte[] message, byte[] iv)
        {

            Crypto.Engines.AesEngine engine = new Crypto.Engines.AesEngine();
            Crypto.Modes.OfbBlockCipher blockCipher = new Crypto.Modes.OfbBlockCipher(engine,128);
            Crypto.Paddings.PaddedBufferedBlockCipher cipher = new Crypto.Paddings.PaddedBufferedBlockCipher(blockCipher); 
            Crypto.Parameters.KeyParameter keyParam = new Crypto.Parameters.KeyParameter(key);
            Crypto.Parameters.ParametersWithIV keyParamWithIV = new Crypto.Parameters.ParametersWithIV(keyParam, iv);

            cipher.Init(true, keyParamWithIV);
            byte[] outputBytes = new byte[cipher.GetOutputSize(message.Length)];
            int length = cipher.ProcessBytes(message, outputBytes, 0);
            cipher.DoFinal(outputBytes, length);

            return outputBytes;
        }

        public byte[] ofbDecoding(byte[] key, byte[] message, byte[] iv)
        {

            Crypto.Engines.AesEngine engine = new Crypto.Engines.AesEngine();
            Crypto.Modes.OfbBlockCipher blockCipher = new Crypto.Modes.OfbBlockCipher(engine, 128); 
            Crypto.Paddings.PaddedBufferedBlockCipher cipher = new Crypto.Paddings.PaddedBufferedBlockCipher(blockCipher); 
            Crypto.Parameters.KeyParameter keyParam = new Crypto.Parameters.KeyParameter(key);
            Crypto.Parameters.ParametersWithIV keyParamWithIV = new Crypto.Parameters.ParametersWithIV(keyParam, iv);

            cipher.Init(false, keyParamWithIV);
            byte[] comparisonBytes = new byte[cipher.GetOutputSize(message.Length)];
            int length = cipher.ProcessBytes(message, comparisonBytes, 0);
            cipher.DoFinal(comparisonBytes, length);

            return comparisonBytes;
        }



        public byte[] aesEncoding(byte[] key, String mode, int blockSize, byte[] message, byte[] iv)
        {
            byte[] encrypted;

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
                    case "CFB":
                        aes.Mode = CipherMode.CFB;
                        break;
                    case "OFB":
                        encrypted = ofbEncoding(key, message, iv);
                        return encrypted;
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
                        case "CFB":
                        aes.Mode = CipherMode.CFB;
                        break;
                    case "OFB":
                        decrypted = ofbDecoding(key, message, iv);
                        return decrypted;
                }
                aes.Padding = PaddingMode.PKCS7;
                ICryptoTransform transform = aes.CreateDecryptor();
                decrypted = transform.TransformFinalBlock(message, 0, message.Length);
                
            }
            return decrypted;
        }

        public byte[] rsaEncoding(byte[] message, RSAParameters publicKey)
        {
            byte[] encrypted;

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(publicKey);
                encrypted = rsa.Encrypt(message, true);
            }

            return encrypted;
        }

        public byte[] rsaDecoding(byte[] message, RSAParameters privateKey)
        {
            byte[] decrypted;

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(privateKey);
                decrypted = rsa.Decrypt(message, true);
            }

            return decrypted;
        }
    }
}

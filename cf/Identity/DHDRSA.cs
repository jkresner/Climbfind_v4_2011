using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace cf.Identity
{
    public class DHDRSA
    {
        private const int CHUNK_SIZE = 80;
        private const int ENCRYPTED_SIZE = 128;
        private const string SEPARATOR = "<---->";

        public static string GenerateKey()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            return rsa.ToXmlString(true);
        }

        public static string GenerateKey(int keySize)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keySize);

            return rsa.ToXmlString(true);
        }

        public static string ExtractPublicKey(string xmlPrivateKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            rsa.FromXmlString(xmlPrivateKey);

            return rsa.ToXmlString(false);
        }

        public static string Encrypt(string xmlPublicKey, string plainData)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            rsa.FromXmlString(xmlPublicKey);

            return Encrypt(rsa, plainData);
        }

        public static string Encrypt(RSACryptoServiceProvider rsa, string plainData)
        {
            Byte[] plainBytes = Encoding.Default.GetBytes(plainData);

            Byte[] encryptedBytes = rsa.Encrypt(plainBytes, true);

            return Encoding.Default.GetString(encryptedBytes);
        }


        public static string Decrypt(string xmlPrivateKey, string encryptedData)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            rsa.FromXmlString(xmlPrivateKey);

            return Decrypt(rsa, encryptedData);
        }

        public static string Decrypt(RSACryptoServiceProvider rsa, string encryptedData)
        {
            Byte[] plainBytes = Encoding.Default.GetBytes(encryptedData);

            Byte[] decryptedBytes = rsa.Decrypt(plainBytes, true);

            return Encoding.Default.GetString(decryptedBytes);
        }


        public static string EncryptByChunks(string xmlPublicKey, string plainData)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPublicKey);


            return EncryptByChunks(rsa, plainData);
        }

        public static string EncryptByChunks(RSACryptoServiceProvider rsa, string plainData)
        {
            int chunks = (int)System.Math.Truncate((decimal)(plainData.Length / CHUNK_SIZE)) + 1;
            string chunkString = string.Empty;

            StringBuilder sb = new StringBuilder();

            int lengthToCopy = 0;

            for (int counter = 0; counter < chunks; counter++)
            {
                if ((counter == chunks - 1) || plainData.Length < CHUNK_SIZE)
                {
                    lengthToCopy = plainData.Length - (counter * CHUNK_SIZE);
                }
                else
                {
                    lengthToCopy = CHUNK_SIZE;
                }

                chunkString = plainData.Substring(counter * CHUNK_SIZE, lengthToCopy);
                sb.Append(Encrypt(rsa, chunkString));
            }

            return sb.ToString();
        }


        public static string DecryptByChunks(string xmlPrivateKey, string encryptedData)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPrivateKey);

            return DecryptByChunks(rsa, encryptedData);
        }

        public static string DecryptByChunks(RSACryptoServiceProvider rsa, string encryptedData)
        {
            //Divide the string into strings of length ENCRYPTED_SIZE
            int chunks = encryptedData.Length / ENCRYPTED_SIZE;
            string chunkString = string.Empty;

            StringBuilder sb = new StringBuilder();

            for (int counter = 0; counter < chunks; counter++)
            {
                chunkString = encryptedData.Substring(counter * ENCRYPTED_SIZE, ENCRYPTED_SIZE);
                sb.Append(Decrypt(rsa, chunkString));
            }

            return sb.ToString();
        }

        public static string EncryptWithSymmetricAid(string xmlPublicKey, string plainData)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPublicKey);

            return EncryptWithSymmetricAid(rsa, plainData);
        }

        public static string EncryptWithSymmetricAid(RSACryptoServiceProvider rsa, string plainData)
        {
            byte[] iv;
            byte[] key;

            //Get a random private key
            getSymmetrycKeys(out iv, out key);

            //Encrypt the text with the symmetric key
            string encryptedMessage = encryptTripleDES(plainData, iv, key);

            //Encrypt the key and iv with the assymetric key
            string simmetricKeys = convertKeysToText(iv, key);
            string encryptedSimmetricKeys = Encrypt(rsa, simmetricKeys); ;

            return encryptedSimmetricKeys + SEPARATOR + encryptedMessage;
        }

        private static string convertKeysToText(byte[] iv, byte[] key)
        {
            string cadenaIV = Convert.ToBase64String(iv);
            string cadenaKey = Convert.ToBase64String(key);

            return cadenaIV + SEPARATOR + cadenaKey;
        }

        public static string DecryptWithSymmetricAid(string xmlPrivateKey, string encryptedData)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPrivateKey);

            return DecryptWithSymmetricAid(rsa, encryptedData);

        }

        public static string DecryptWithSymmetricAid(RSACryptoServiceProvider rsa, string encryptedData)
        {
            string[] data = splitByString(encryptedData, SEPARATOR);

            string encryptedSymmetricKey = data[0];
            string encryptedMessage = data[1];

            //Decrypt the symmetric key
            string symmetricKey = Decrypt(rsa, encryptedSymmetricKey);

            byte[] iv = Convert.FromBase64String(splitByString(symmetricKey, SEPARATOR)[0]);
            byte[] key = Convert.FromBase64String(splitByString(symmetricKey, SEPARATOR)[1]);

            string message = decryptTripleDES(encryptedMessage, iv, key);

            return message;

        }

        private static string[] splitByString(string value, string separator)
        {
            int separatorPos = value.IndexOf(SEPARATOR);

            string[] returnValue = new string[2];

            returnValue[0] = value.Substring(0, separatorPos);
            returnValue[1] = value.Substring(separatorPos + separator.Length);

            return returnValue;
        }

        public static void getSymmetrycKeys(out byte[] iv, out byte[] key)
        {
            TripleDESCryptoServiceProvider cryptoProvider = new TripleDESCryptoServiceProvider();
            KeySizes[] ks = cryptoProvider.LegalKeySizes;
            iv = cryptoProvider.IV;
            key = cryptoProvider.Key;
        }


        public static string encryptTripleDES(string value, byte[] iv, byte[] key)
        {
            string retorno = string.Empty;

            if ((value != ""))
            {
                TripleDESCryptoServiceProvider cryptoProvider = new TripleDESCryptoServiceProvider();
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, cryptoProvider.CreateEncryptor(key, iv), CryptoStreamMode.Write);
                StreamWriter sw = new StreamWriter(cs);
                sw.Write(value);
                sw.Flush();
                cs.FlushFinalBlock();
                ms.Flush();
                retorno = Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
            }

            return retorno;
        }

        public static string decryptTripleDES(string value, byte[] iv, byte[] key)
        {
            string retorno = string.Empty;

            if ((value != ""))
            {
                TripleDESCryptoServiceProvider cryptoProvider = new TripleDESCryptoServiceProvider();

                byte[] buffer = Convert.FromBase64String(value);
                MemoryStream ms = new MemoryStream(buffer);
                CryptoStream cs = new CryptoStream(ms, cryptoProvider.CreateDecryptor(key, iv), CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cs);
                retorno = sr.ReadToEnd();
            }
            return retorno;
        }

    }
}

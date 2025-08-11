/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.17 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Data;

namespace Sineva.VHL.Library
{
    public class XEncryption
    {
        public string AESEncrypt256(string input, string key)
        {
            ///
            key = string.Empty;
            for (int i = 0; i < 32; i++)
            {
                key += (char)('a' + i % 20);
            }
            ///
            string output = string.Empty;

            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            byte[] temp = Encoding.UTF8.GetBytes(key);
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            //var encryptor = aes.CreateEncryptor();
            byte[] xBuf = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    byte[] xXml = Encoding.UTF8.GetBytes(input);
                    cs.Write(xXml, 0, xXml.Length);
                }

                xBuf = ms.ToArray();
            }

            output = Convert.ToBase64String(xBuf);
            return output;
        }

        public string AESDecrypt256(string input, string key)
        {
            string output = string.Empty;

            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            var decryptor = aes.CreateDecryptor();
            byte[] xBuf = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                {
                    byte[] xXml = Convert.FromBase64String(input);
                    cs.Write(xXml, 0, xXml.Length);
                }

                xBuf = ms.ToArray();
            }

            output = Encoding.UTF8.GetString(xBuf);
            return output;
        }

        public static string AESEncrypt128(string input, string key)
        {
            string output = string.Empty;

            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            byte[] plainText = Encoding.Unicode.GetBytes(input);
            byte[] salt = Encoding.ASCII.GetBytes(key.Length.ToString());

            PasswordDeriveBytes secretKey = new PasswordDeriveBytes(key, salt);
            ICryptoTransform encryptor = rijndaelCipher.CreateEncryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));

            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

            cryptoStream.Write(plainText, 0, plainText.Length);
            cryptoStream.FlushFinalBlock();

            byte[] cipherBytes = memoryStream.ToArray();

            memoryStream.Close();
            cryptoStream.Close();

            output = Convert.ToBase64String(cipherBytes);
            return output;
        }

        public static string AESDecrypt128(string input, string key)
        {
            string output = string.Empty;

            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            byte[] encryptedData = Convert.FromBase64String(input);
            byte[] salt = Encoding.ASCII.GetBytes(key.Length.ToString());

            PasswordDeriveBytes secretKey = new PasswordDeriveBytes(key, salt);
            ICryptoTransform decryptor = rijndaelCipher.CreateDecryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));

            MemoryStream memoryStream = new MemoryStream(encryptedData);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

            byte[] plainText = new byte[encryptedData.Length];

            int decryptedCount = cryptoStream.Read(plainText, 0, plainText.Length);

            memoryStream.Close();
            cryptoStream.Close();

            output = Encoding.Unicode.GetString(plainText, 0, decryptedCount);
            return output;
        }

        public static string RSAEncrypt(string input, string pubKey)
        {
            string output = string.Empty;

            byte[] keybuf = Convert.FromBase64String(pubKey);
            pubKey = (new UTF8Encoding()).GetString(keybuf);

            RSACryptoServiceProvider encryptor = new RSACryptoServiceProvider();
            encryptor.FromXmlString(pubKey);

            byte[] plainBuf = (new UTF8Encoding()).GetBytes(input);
            byte[] encBuf = encryptor.Encrypt(plainBuf, false);

            output = Convert.ToBase64String(encBuf);
            return output;
        }

        public static byte[] RSAEncrypt(byte[] data, RSAParameters RSAKeyInfo, bool DoOAEPadding)
        {
            try
            {
                byte[] encryptedData;

                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(RSAKeyInfo);

                encryptedData = rsa.Encrypt(data, DoOAEPadding);
                return encryptedData;
            }
            catch (CryptographicException err)
            {
                return null;
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public static string RSADecrypt(string input, string prvKey)
        {
            string output = string.Empty;

            byte[] keybuf = Convert.FromBase64String(prvKey);
            prvKey = (new UTF8Encoding()).GetString(keybuf);

            RSACryptoServiceProvider decryptor = new RSACryptoServiceProvider();
            decryptor.FromXmlString(prvKey);

            byte[] encryptedBuf = Convert.FromBase64String(input);
            byte[] decryptedBuf = decryptor.Decrypt(encryptedBuf, false);

            output = (new UTF8Encoding()).GetString(decryptedBuf, 0, decryptedBuf.Length);
            return output;
        }

        public static byte[] RSADecrypt(byte[] data, RSAParameters RSAKeyInfo, bool DoOAEPadding)
        {
            try
            {
                byte[] decryptedData;

                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(RSAKeyInfo);

                decryptedData = rsa.Decrypt(data, DoOAEPadding);
                return decryptedData;
            }
            catch (CryptographicException err)
            {
                return null;
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public RSACryptoServiceProvider GenerateKey(string containerName)
        {
            CspParameters cp = new CspParameters();
            cp.Flags = CspProviderFlags.UseMachineKeyStore | CspProviderFlags.UseArchivableKey;
            cp.KeyContainerName = containerName;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cp);
            return rsa;
        }

        public static bool RemoveKeyFromContainer(string containerName)
        {
            bool rv = false;

            try
            {
                CspParameters cp = new CspParameters();
                cp.KeyContainerName = containerName;

                // Create a new instance of RSACryptoServiceProvider that accesses
                // the key container.
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cp);

                // Delete the key entry in the container.
                rsa.PersistKeyInCsp = false;

                // Call Clear to release resources and delete the key from the container.
                rsa.Clear();

                return true;
            }
            catch (Exception err)
            {
                return false;
            }
        }

        public void EncryptDataSet(DataSet ds)
        {
            // Create the DES encryption provider:
            System.Security.Cryptography.DES des = new System.Security.Cryptography.DESCryptoServiceProvider();
            // Serialize the DES provider's key and IV to disk for decryption later:
            using (StreamWriter sw = new StreamWriter("DES.bin"))
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                byte[][] stuff = new byte[2][];
                stuff[0] = des.Key;
                stuff[1] = des.IV;
                bf.Serialize(sw.BaseStream, stuff);
            }

            using (FileStream fs = new FileStream("DataSet.bin", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (System.Security.Cryptography.CryptoStream cs =
                    new System.Security.Cryptography.CryptoStream(fs, des.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write))
                {
                    // Encrypt the DataSet to the file:
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(ds.GetXml());
                    }
                }
            }

            // Now write the DataSet schema to disk:
            ds.WriteXmlSchema("DataSet.xsd");
        }

        public string GetMd5Hash(byte[] input)
        {
            MD5 hasher = MD5.Create();
            byte[] hashedBytes = hasher.ComputeHash(input);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashedBytes.Length; i++)
            {
                sb.Append(hashedBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public string GetEncryptedPassword(string userId, string password)
        {
            string rv = string.Empty;
            try
            {
                byte[] salt = CreatePasswordSalt(userId, password);

                Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, salt, 10000);
                byte[] discard = rfc.GetBytes(userId.Length + password.Length);
                byte[] pwd = rfc.GetBytes(discard.Length);

                rv = GetMd5Hash(pwd);
            }
            catch (Exception err)
            {

            }
            return rv;
        }

        private byte[] CreatePasswordSalt(string account, string password)
        {
            if (string.IsNullOrEmpty(password)) password = "default password text";
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < password.Length; i++)
            {
                sb.Append(password.Substring(i, 1) + account.Substring(i % account.Length, 1));
            }
            byte[] combined = Encoding.Unicode.GetBytes(sb.ToString());

            int sum = 0;
            for (int i = 0; i < combined.Length; i++)
            {
                sum += combined[i];
            }

            string fractional = (Math.Log(combined.Length, sum) % 1).ToString().Remove(0, 2);
            int length = fractional.Length;
            if (length < 8) length = 8;

            byte[] salt = new byte[length];
            byte[] temp = Encoding.Unicode.GetBytes(fractional);
            int index = 0;
            for (int i = 0; i < temp.Length; i++)
            {
                if (index >= salt.Length) break;

                if (temp[i] > 0)
                    salt[index++] = temp[i];
            }

            return salt;
        }

        private byte[] CreateSalt(int length, string key)
        {
            //if(length <= 0 || length > 10) length = 10;

            if (length < 8) length = 10;
            byte[] bytes = new byte[length];

            for (int i = 0; i < bytes.Length; i++)
            {
                int index = i % key.Length;
                byte[] temp = Encoding.Unicode.GetBytes(key.Substring(index));

                int max = Byte.MinValue;
                int min = Byte.MaxValue;
                for (int j = 0; j < sizeof(int); j++)
                {
                    if (j >= temp.Length)
                    {
                        min = max / 2;
                        break;
                    }
                    if (temp[j] <= 0) continue;
                    if (max < temp[j]) max = temp[j];
                    if (min > temp[j]) min = temp[j];
                }
                bytes[i] = Convert.ToByte(max - min);

                //////////////////////////
                string msg = "Key : ";
                for (int j = 0; j < temp.Length; j++)
                {
                    msg += temp[j].ToString() + " ";
                }
                msg += string.Format("\nSalt : {0}({1},{2})", bytes[i], max, min);
                // System.Windows.Forms.MessageBox.Show(msg);
            }

            return bytes;
        }
    }
}

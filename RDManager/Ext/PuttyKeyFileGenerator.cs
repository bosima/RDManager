using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto.Parameters;

// From: https://github.com/akira345/pem2ppk , Thanks akira345.

//https://antonymale.co.uk/generating-putty-key-files.htmlより一部修正

namespace PEM2PPK
{
    public class PuttyKeyFileGenerator
    {
        private const int prefixSize = 4;
        private const int paddedPrefixSize = prefixSize + 1;
        private const int lineLength = 64;
        private const string keyType = "ssh-rsa";

        public static string RSAToPuttyPrivateKey(RSAParameters keyParameters, string passPharse, string comment = "imported-openssh-key")
        {
            //RSACryptoServiceProvider からRSAParametersに変更
            //RSAParameters keyParameters = cryptoServiceProvider.ExportParameters(includePrivateParameters: true);

            // create public buffer
            byte[] publicBuffer = new byte[3 + keyType.Length + GetPrefixSize(keyParameters.Exponent) + keyParameters.Exponent.Length +
                GetPrefixSize(keyParameters.Modulus) + keyParameters.Modulus.Length + 1];

            using (var writer = new BinaryWriter(new MemoryStream(publicBuffer), Encoding.ASCII))
            {
                writer.Write(new byte[] { 0x00, 0x00, 0x00 });
                writer.Write(keyType);
                WritePrefixed(writer, keyParameters.Exponent, CheckIsNeddPadding(keyParameters.Exponent));
                WritePrefixed(writer, keyParameters.Modulus, CheckIsNeddPadding(keyParameters.Modulus));
            }

            // create private buffer
            byte[] privateBuffer = new byte[GetPrefixSize(keyParameters.D) + keyParameters.D.Length + GetPrefixSize(keyParameters.P) + keyParameters.P.Length +
                                            GetPrefixSize(keyParameters.Q) + keyParameters.Q.Length + GetPrefixSize(keyParameters.InverseQ) + keyParameters.InverseQ.Length];

            using (var writer = new BinaryWriter(new MemoryStream(privateBuffer), Encoding.ASCII))
            {
                WritePrefixed(writer, keyParameters.D, CheckIsNeddPadding(keyParameters.D));
                WritePrefixed(writer, keyParameters.P, CheckIsNeddPadding(keyParameters.P));
                WritePrefixed(writer, keyParameters.Q, CheckIsNeddPadding(keyParameters.Q));
                WritePrefixed(writer, keyParameters.InverseQ, CheckIsNeddPadding(keyParameters.InverseQ));
            }

            string encryptionType = "none";
            int cipherblk = 1;
            if (!string.IsNullOrWhiteSpace(passPharse))
            {
                encryptionType = "aes256-cbc";
                cipherblk = 16;
            }

            // create the MAC
            int privateEncryptedBufferLength = privateBuffer.Length + cipherblk - 1;
            privateEncryptedBufferLength -= privateEncryptedBufferLength % cipherblk;
            byte[] privateEncryptedBuffer = new byte[privateEncryptedBufferLength];
            using (var writer = new BinaryWriter(new MemoryStream(privateEncryptedBuffer), Encoding.ASCII))
            {
                writer.Write(privateBuffer);

                if (privateEncryptedBufferLength > privateBuffer.Length)
                {
                    Debug.Assert(privateEncryptedBufferLength - privateBuffer.Length < 20);
                    using (var sha1 = new SHA1CryptoServiceProvider())
                    {
                        byte[] privateHash = sha1.ComputeHash(privateBuffer);
                        writer.Write(privateHash, 0, privateEncryptedBufferLength - privateBuffer.Length);
                    }
                }
            }

            byte[] bytesToHash = new byte[prefixSize + keyType.Length + prefixSize + encryptionType.Length + prefixSize + comment.Length +
                                          prefixSize + publicBuffer.Length + prefixSize + privateEncryptedBuffer.Length];

            using (var writer = new BinaryWriter(new MemoryStream(bytesToHash)))
            {
                WritePrefixed(writer, Encoding.ASCII.GetBytes(keyType));
                WritePrefixed(writer, Encoding.ASCII.GetBytes(encryptionType));
                WritePrefixed(writer, Encoding.ASCII.GetBytes(comment));
                WritePrefixed(writer, publicBuffer);
                WritePrefixed(writer, privateEncryptedBuffer);
            }

            string macKeyStr = "putty-private-key-file-mac-key";
            if (!string.IsNullOrWhiteSpace(passPharse))
            {
                macKeyStr += passPharse;
            }

            byte[] macKey;
            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                macKey = sha1.ComputeHash(Encoding.ASCII.GetBytes(macKeyStr));
            }

            string mac;
            using (var hmacsha1 = new HMACSHA1(macKey))
            {
                mac = string.Join("", hmacsha1.ComputeHash(bytesToHash).Select(x => string.Format("{0:x2}", x)));
            }
            
            // encrypt private blob
            if (!string.IsNullOrWhiteSpace(passPharse))
            {
                byte[] passBytes = Encoding.ASCII.GetBytes(passPharse);
                int passBufferLength = passPharse.Length + 4;

                byte[] passBuffer1 = new byte[passBufferLength];
                Buffer.BlockCopy(passBytes, 0, passBuffer1, 4, passBytes.Length);
                byte[] passKey1;
                using (var sha1 = new SHA1CryptoServiceProvider())
                {
                    passKey1 = new SHA1CryptoServiceProvider().ComputeHash(passBuffer1);
                }

                byte[] passBuffer2 = new byte[passBufferLength];
                passBuffer2[3] = 1;
                Buffer.BlockCopy(passBytes, 0, passBuffer2, 4, passBytes.Length);
                byte[] passKey2;
                using (var sha1 = new SHA1CryptoServiceProvider())
                {
                    passKey2 = new SHA1CryptoServiceProvider().ComputeHash(passBuffer2);
                }

                byte[] passKey = new byte[40];
                Buffer.BlockCopy(passKey1, 0, passKey, 0, 20);
                Buffer.BlockCopy(passKey2, 0, passKey, 20, 20);

                byte[] iv = new byte[16];
                byte[] aesKey = new byte[32];
                Buffer.BlockCopy(passKey, 0, aesKey, 0, 32);
                privateEncryptedBuffer = AES256Encrypt(aesKey, iv, privateEncryptedBuffer);
            }

            // output
            var sb = new StringBuilder();
            sb.AppendLine("PuTTY-User-Key-File-2: " + keyType);
            sb.AppendLine("Encryption: " + encryptionType);
            sb.AppendLine("Comment: " + comment);

            string publicBlob = Convert.ToBase64String(publicBuffer);
            var publicLines = SpliceText(publicBlob, lineLength).ToArray();
            sb.AppendLine("Public-Lines: " + publicLines.Length);
            foreach (var line in publicLines)
            {
                sb.AppendLine(line);
            }

            string privateBlob = Convert.ToBase64String(privateEncryptedBuffer);
            var privateLines = SpliceText(privateBlob, lineLength).ToArray();
            sb.AppendLine("Private-Lines: " + privateLines.Length);
            foreach (var line in privateLines)
            {
                sb.AppendLine(line);
            }

            sb.AppendLine("Private-MAC: " + mac);

            return sb.ToString();
        }

        private static void WritePrefixed(BinaryWriter writer, byte[] bytes, bool addLeadingNull = false)
        {
            var length = bytes.Length;
            if (addLeadingNull)
                length++;

            if (BitConverter.IsLittleEndian)
            {
                writer.Write(BitConverter.GetBytes(length).Reverse().ToArray());
            }
            else
            {
                writer.Write(BitConverter.GetBytes(length));
            }
           
            if (addLeadingNull)
                writer.Write((byte)0x00);
            writer.Write(bytes);
        }

        private static IEnumerable<string> SpliceText(string text, int length)
        {
            for (int i = 0; i < text.Length; i += length)
            {
                yield return text.Substring(i, Math.Min(length, text.Length - i));
            }
        }

        private static bool CheckIsNeddPadding(byte[] bytes)
        {
            // 128 == 10000000
            // This means that the number of bits can be divided by 8.
            // According to the algorithm in putty, you need to add a padding.
            return bytes[0] >= 128;
        }

        private static int GetPrefixSize(byte[] bytes)
        {
            return CheckIsNeddPadding(bytes) ? paddedPrefixSize : prefixSize;
        }

        private static byte[] AES256Encrypt(byte[] key, byte[] iv, byte[] bytes)
        {
            using (RijndaelManaged rijalg = new RijndaelManaged())
            {
                rijalg.BlockSize = 128;
                rijalg.KeySize = 256;
                rijalg.Padding = PaddingMode.None;
                rijalg.Mode = CipherMode.CBC;
                rijalg.Key = key;
                rijalg.IV = iv;

                ICryptoTransform encryptor = rijalg.CreateEncryptor(rijalg.Key, rijalg.IV);
                return encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
            }
        }
    }
}

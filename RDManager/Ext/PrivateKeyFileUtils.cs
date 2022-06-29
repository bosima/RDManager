using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using PEM2PPK;

namespace RDManager
{
    internal class PrivateKeyFileUtils
    {
        public static void PemToPpk(string pemFilePath, string keyPassPhrase)
        {
            using (TextReader sr = new StreamReader(pemFilePath, Encoding.ASCII))
            {
                PemReader pr;
                if (!string.IsNullOrWhiteSpace(keyPassPhrase))
                {
                    var pf = new PasswordFinder(keyPassPhrase);
                    pr = new PemReader(sr, pf);
                }
                else
                {
                    pr = new PemReader(sr);
                }

                AsymmetricCipherKeyPair keyPair = (AsymmetricCipherKeyPair)pr.ReadObject();

                RSAParameters rsa = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)keyPair.Private);
                var ppk = PuttyKeyFileGenerator.RSAToPuttyPrivateKey(rsa, keyPassPhrase);
                var ppkPath = pemFilePath.Substring(0, pemFilePath.LastIndexOf(".")) + ".ppk";
                File.WriteAllText(ppkPath, ppk);
            }
        }

        public static void EncryptPem(string pemFilePath, string password, string newPassword)
        {
            AsymmetricCipherKeyPair keyPair;
            using (TextReader sr = new StreamReader(pemFilePath, Encoding.ASCII))
            {
                PemReader pr;
                if (!string.IsNullOrWhiteSpace(password))
                {
                    var pf = new PasswordFinder(password);
                    pr = new PemReader(sr, pf);
                }
                else
                {
                    pr = new PemReader(sr);
                }

                keyPair = (AsymmetricCipherKeyPair)pr.ReadObject();
            }

            if (File.Exists(pemFilePath))
            {
                File.Delete(pemFilePath);
            }

            using (TextWriter textWriter = new StreamWriter(pemFilePath, false, Encoding.ASCII))
            {
                var pemWriter = new PemWriter(textWriter);
                // DESEDE、AES-256-CBC
                pemWriter.WriteObject(keyPair.Private, "AES-256-CBC", newPassword.ToCharArray(), new SecureRandom());
                pemWriter.Writer.Flush();
            }
        }

        public static string UploadFile(string localFilePath)
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var ppkPath = Path.Combine(basePath, "keys");
            if (!Directory.Exists(ppkPath))
            {
                Directory.CreateDirectory(ppkPath);
            }
            var ppkFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".pem";
            var destFilePath = Path.Combine(ppkPath, ppkFileName);
            File.Copy(localFilePath, destFilePath, true);
            return ppkFileName;
        }

        public static string GetPpkFileName(string pemFileName, string keyPassPhrase)
        {
            var ppkFileName = pemFileName.Substring(0, pemFileName.LastIndexOf(".")) + ".ppk";
            var ppksPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "keys", ppkFileName);
            if (!File.Exists(ppksPath))
            {
                var pemPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "keys", pemFileName);
                PemToPpk(pemPath, keyPassPhrase);
            }

            return ppkFileName;
        }

        public static void RemoveFile(string pemFileName)
        {
            var pemFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "keys", pemFileName);
            if (File.Exists(pemFilePath))
            {
                File.Delete(pemFilePath);
            }

            var ppkFileName = pemFileName.Substring(0, pemFileName.LastIndexOf(".")) + ".ppk";
            var ppkFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "keys", ppkFileName);
            if (File.Exists(ppkFilePath))
            {
                File.Delete(ppkFilePath);
            }
        }
    }
}

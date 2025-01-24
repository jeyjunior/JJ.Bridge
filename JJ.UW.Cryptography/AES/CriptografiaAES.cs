using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace JJ.UW.Cryptography.AES
{
    internal static class CriptografiaAES
    {
        public static CriptografarResult Criptografar(CriptografarRequest criptografarRequest)
        {
            var criptografiaResult = new CriptografarResult();

            using (Aes aesAlg = Aes.Create()) 
            {
                aesAlg.Key = ObterChaveAES();

                if (string.IsNullOrEmpty(criptografarRequest.IV))
                    aesAlg.GenerateIV();
                else
                    aesAlg.IV = Convert.FromBase64String(criptografarRequest.IV);

                criptografiaResult.IV = Convert.ToBase64String(aesAlg.IV);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream ms = new MemoryStream()) 
                { 
                    using(CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs)) 
                        { 
                            sw.Write(criptografarRequest.Valor);
                        }
                    }

                    criptografiaResult.Valor = Convert.ToBase64String(ms.ToArray());
                }
            }
            
            return criptografiaResult;
        }

        public static DescriptografarResult Descriptografar(DescriptografarRequest descriptografarRequest)
        {
            var result = new DescriptografarResult { Valor = "", Erro = "" };

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = ObterChaveAES();
                aesAlg.IV = Convert.FromBase64String(descriptografarRequest.IV);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(descriptografarRequest.Valor)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            result.Valor = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return result;
        }

        private static byte[] ObterChaveAES()
        {
            string chave = ObterChave();

            using (SHA256 sha256 = SHA256.Create())
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(chave));
        }

        private static string ObterChave()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var arquivoKeyMaster = Path.Combine(localFolder.Path, "KeyMaster.txt");

            if (File.Exists(arquivoKeyMaster))
            {
                string guidString = File.ReadAllText(arquivoKeyMaster);
                return Guid.Parse(guidString).ToString();
            }
            else
            {
                Guid novoGuid = Guid.NewGuid();
                File.WriteAllText(arquivoKeyMaster, novoGuid.ToString());
                return novoGuid.ToString();
            }
        }
    }
}

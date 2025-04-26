using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JJ.NET.Cryptography.AES
{
    internal static class CriptografiaAES
    {
        public static CriptografiaResult Criptografar(CriptografiaRequest criptografarRequest)
        {
            var criptografiaResult = new CriptografiaResult();

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
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
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

        public static CriptografiaResult Descriptografar(CriptografiaRequest criptografiaRequest)
        {
            var result = new CriptografiaResult { Valor = "", Erro = "" };

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = ObterChaveAES();
                aesAlg.IV = Convert.FromBase64String(criptografiaRequest.IV);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(criptografiaRequest.Valor)))
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
            var caminho = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cclrf.dat");
            var chave = LerChaveLocal(caminho);

            if (string.IsNullOrWhiteSpace(chave))
            {
                chave = Guid.NewGuid().ToString();
                SalvarChaveLocal(chave, caminho);
            }

            return chave;
        }

        private static void SalvarChaveLocal(string chave, string caminho)
        {
            try
            {
                byte[] dados = Encoding.UTF8.GetBytes(chave);
                byte[] dadosCriptografados = ProtectedData.Protect(dados, null, DataProtectionScope.CurrentUser);
                File.WriteAllBytes(caminho, dadosCriptografados);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao salvar a chave mestra: " + ex.Message);
            }
        }

        private static string LerChaveLocal(string caminho)
        {
            try
            {
                if (!File.Exists(caminho))
                    return "";

                byte[] dadosCriptografados = File.ReadAllBytes(caminho);
                byte[] dados = ProtectedData.Unprotect(dadosCriptografados, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(dados);
            }
            catch
            {
                return "";
            }
        }
    }
}

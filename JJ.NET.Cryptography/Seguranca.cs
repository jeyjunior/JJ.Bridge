using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using JJ.NET.Cryptography.DTO;
using JJ.NET.Cryptography.Interfaces;

namespace JJ.NET.Cryptography
{
    public class Seguranca :  ISeguranca
    {
        private const string KMFileName = "cclrf.dat";
        private readonly string KMPath = Path.Combine(Path.GetTempPath(), KMFileName);
        private const int KeySize = 256;
        private const int SaltSize = 32;
        private const int Iterations = 100000;

        private string GerarSalt()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var saltBytes = new byte[SaltSize];
                rng.GetBytes(saltBytes);
                return Convert.ToBase64String(saltBytes);
            }
        }
        public string GerarChavePrincipal()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var kmBytes = new byte[KeySize];
                rng.GetBytes(kmBytes);
                var km = Convert.ToBase64String(kmBytes);

                // Salva no arquivo temporário
                File.WriteAllText(KMPath, km);

                return km; // Retorna para backup do usuário
            }
        }
        public string RegistrarChavePrincipal(string km)
        {
            try
            {
                // Valida se a KM tem o formato/tamanho correto
                var kmBytes = Convert.FromBase64String(km);
                if (kmBytes.Length != KeySize)
                    throw new ArgumentException("Chave principal inválida");

                // Salva no arquivo temporário
                File.WriteAllText(KMPath, km);

                return km;
            }
            catch (FormatException)
            {
                throw new ArgumentException("Formato da chave principal inválido");
            }
        }
        private byte[] ObterKM()
        {
            if (!File.Exists(KMPath))
                throw new FileNotFoundException("Chave principal não encontrada");

            var kmBase64 = File.ReadAllText(KMPath);
            return Convert.FromBase64String(kmBase64);
        }
        public CriptografiaResult Criptografar(string valor)
        {
            var km = ObterKM();
            var salt = GerarSalt();
            var saltBytes = Encoding.UTF8.GetBytes(salt);

            // Deriva uma chave específica para este registro
            using (var pbkdf2 = new Rfc2898DeriveBytes(km, saltBytes, Iterations, HashAlgorithmName.SHA256))
            {
                var derivedKey = pbkdf2.GetBytes(KeySize);

                using (var aes = Aes.Create())
                {
                    aes.Key = derivedKey;
                    aes.GenerateIV();

                    using (var encryptor = aes.CreateEncryptor())
                    using (var ms = new MemoryStream())
                    {
                        // Escreve o IV primeiro
                        ms.Write(aes.IV, 0, aes.IV.Length);

                        // Criptografa os dados
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        using (var sw = new StreamWriter(cs))
                        {
                            sw.Write(valor);
                        }

                        return new CriptografiaResult
                        {
                            ValorCriptografado = Convert.ToBase64String(ms.ToArray()),
                            Salt = salt
                        };
                    }
                }
            }
        }
        public string Descriptografar(DescriptografiaRequest descriptografiaRequest)
        {
            var km = ObterKM();
            var saltBytes = Encoding.UTF8.GetBytes(descriptografiaRequest.Salt);
            var encryptedData = Convert.FromBase64String(descriptografiaRequest.ValorCriptografado);

            // Deriva a mesma chave usada na criptografia
            using (var pbkdf2 = new Rfc2898DeriveBytes(km, saltBytes, Iterations, HashAlgorithmName.SHA256))
            {
                var derivedKey = pbkdf2.GetBytes(KeySize);

                using (var aes = Aes.Create())
                {
                    // Extrai o IV dos primeiros 16 bytes
                    var iv = new byte[16];
                    Array.Copy(encryptedData, 0, iv, 0, iv.Length);
                    aes.IV = iv;
                    aes.Key = derivedKey;

                    // O restante são os dados criptografados
                    var cipherText = new byte[encryptedData.Length - iv.Length];
                    Array.Copy(encryptedData, iv.Length, cipherText, 0, cipherText.Length);

                    using (var decryptor = aes.CreateDecryptor())
                    using (var ms = new MemoryStream(cipherText))
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    using (var sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
    }
}


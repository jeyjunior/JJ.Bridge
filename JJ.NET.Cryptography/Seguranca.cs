using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using JJ.NET.Cryptography.DTO;
using JJ.NET.Cryptography.Interfaces;
using System.Text.Json;

namespace JJ.NET.Cryptography
{
    public class Seguranca :  ISeguranca
    {
        private const string KMFileName = "cclrf.dat";
        private readonly string KMPath = Path.Combine(Path.GetTempPath(), KMFileName);
        private const int KeySizeInBits = 256; // 256 bits = 32 bytes
        private const int KeySizeInBytes = KeySizeInBits / 8; // 32 bytes
        private const int SaltSize = 32; // 32 bytes = 256 bits
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
        public string GerarChavePrincipal(int idUsuario)
        {
            string kmConcatenado = "";

            using (var rng = RandomNumberGenerator.Create())
            {
                var kmBytes = new byte[KeySizeInBytes];
                rng.GetBytes(kmBytes);
                var km = Convert.ToBase64String(kmBytes);

                var lista = LerKMCriptografado();

                var kmEntry = new KMEntry { UUID = new Guid(), IDUsuario = idUsuario, KM = km };
                lista.Add(kmEntry);
                SalvarKMCriptografado(lista);

                kmConcatenado = kmEntry.UUID + ":" + kmEntry.IDUsuario + ":" + kmEntry.KM;

                return kmConcatenado;
            }
        }
        public bool ValidarChavePrincipal(string chavePrincipal)
        {
            if (string.IsNullOrWhiteSpace(chavePrincipal))
                return false;

            var partes = chavePrincipal.Split(':');
            if (partes.Length != 3)
                return false;

            if (!Guid.TryParse(partes[0], out Guid uuid))
                return false;

            if (!int.TryParse(partes[1], out int idUsuario))
                return false;

            string km = partes[2];

            var lista = LerKMCriptografado();

            return lista.Any(e =>
                e.UUID == uuid &&
                e.IDUsuario == idUsuario &&
                e.KM == km);
        }
        private byte[] ObterKM(int idUsuario)
        {
            var lista = LerKMCriptografado();

            var kmEncontrado = lista.FirstOrDefault(i => i.IDUsuario == idUsuario);

            if (kmEncontrado == null)
                throw new Exception("Não foi possível obter as informações para criptografia/descriptografia.");

            return Convert.FromBase64String(kmEncontrado.KM);
        }

        public CriptografiaResult Criptografar(string valor, int idUsuario)
        {
            var km = ObterKM(idUsuario);
            var salt = GerarSalt();
            var saltBytes = Encoding.UTF8.GetBytes(salt);

            // Deriva uma chave específica para este registro
            using (var pbkdf2 = new Rfc2898DeriveBytes(km, saltBytes, Iterations, HashAlgorithmName.SHA256))
            {
                var derivedKey = pbkdf2.GetBytes(KeySizeInBytes);

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
            var km = ObterKM(descriptografiaRequest.IDUsuario);
            var saltBytes = Encoding.UTF8.GetBytes(descriptografiaRequest.Salt);
            var encryptedData = Convert.FromBase64String(descriptografiaRequest.ValorCriptografado);

            // Deriva a mesma chave usada na criptografia
            using (var pbkdf2 = new Rfc2898DeriveBytes(km, saltBytes, Iterations, HashAlgorithmName.SHA256))
            {
                var derivedKey = pbkdf2.GetBytes(KeySizeInBytes);

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
        private void SalvarKMCriptografado(List<KMEntry> kMs)
        {
            var json = JsonSerializer.Serialize(kMs);
            var plainBytes = Encoding.UTF8.GetBytes(json);

            using (var aes = Aes.Create())
            {
                aes.Key = ObterChaveFixaParaKM();
                aes.GenerateIV();

                using var ms = new MemoryStream();
                ms.Write(aes.IV, 0, aes.IV.Length);

                using var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(plainBytes, 0, plainBytes.Length);
                cs.FlushFinalBlock();

                File.WriteAllBytes(KMPath, ms.ToArray());
            }
        }
        private List<KMEntry> LerKMCriptografado()
        {
            if (!File.Exists(KMPath))
                return new List<KMEntry>();

            var data = File.ReadAllBytes(KMPath);

            using var aes = Aes.Create();
            aes.Key = ObterChaveFixaParaKM();

            var iv = data.Take(16).ToArray();
            var cipher = data.Skip(16).ToArray();
            aes.IV = iv;

            using var ms = new MemoryStream(cipher);
            using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            var json = sr.ReadToEnd();
            return JsonSerializer.Deserialize<List<KMEntry>>(json);
        }
        private byte[] ObterChaveFixaParaKM()
        {
            var chave = "B7g@x!92aBc#Vf5$1xZkLm8*7qQw3pPl";
            return Encoding.UTF8.GetBytes(chave);
        }
    }
}


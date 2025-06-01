using JJ.Net.Cryptography.DTO;
using JJ.Net.Cryptography.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JJ.Net.Cryptography
{
    public class Seguranca : ISeguranca
    {
        private const string KMFileName = "cclrf.json";
        private readonly string KMPath = "";
        private const int KeySizeInBits = 256; // 256 bits = 32 bytes
        private const int KeySizeInBytes = KeySizeInBits / 8; // 32 bytes
        private const int SaltSize = 32; // 32 bytes = 256 bits
        private const int Iterations = 100000;

        public Seguranca(string pastaArmazenamentoSistema)
        {
            KMPath = pastaArmazenamentoSistema;
        }

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

                var lista = LerChavePrincipal();

                var kmEntry = new KMEntry { UUID = Guid.NewGuid(), IDUsuario = idUsuario, KM = km };
                lista.Add(kmEntry);
                SalvarChavePrincpal(lista);

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

            var lista = LerChavePrincipal();

            return lista.Any(e =>
                e.UUID == uuid &&
                e.IDUsuario == idUsuario &&
                e.KM == km);
        }
        private byte[] ObterKM(int idUsuario)
        {
            var lista = LerChavePrincipal();

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
        private void SalvarChavePrincpal(List<KMEntry> kMs)
        {
            var json = JsonSerializer.Serialize(kMs, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(KMPath, json);
        }
        private List<KMEntry> LerChavePrincipal()
        {
            if (!File.Exists(KMPath))
                return new List<KMEntry>();

            var json = File.ReadAllText(KMPath);
            return JsonSerializer.Deserialize<List<KMEntry>>(json) ?? new List<KMEntry>();
        }
    }
}

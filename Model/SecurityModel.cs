using Common;
using Common.Param;
using Common.Result;

using Microsoft.Extensions.Configuration;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Model
{
    public static class SecurityModel
    {
        public static async Task<T> DecryptedObjectAsync<T>(SecureParam secureParam, string privateKey) where T : BaseParam
        {
            return await Task.Run(() =>
            {
                var keyDecrypted = DecryptRsa(secureParam.Key, privateKey);
                var dataDecrypted = DecryptAes(secureParam.Data, keyDecrypted);
                return JsonSerializer.Deserialize<T>(dataDecrypted)!;
            });
        }
        public static async Task<string> GetHash512Async(byte[] data)
        {
            return await Task.Run(() =>
            {
                using (SHA512 sha512 = SHA512.Create())
                {
                    byte[] hashBytes = sha512.ComputeHash(data);

                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in hashBytes)
                    {
                        sb.Append(b.ToString("x2"));
                    }

                    return sb.ToString();
                }
            });
        }
        public static async Task<string> GetHash512Async(string data)
        {
            return await GetHash512Async(Encoding.UTF8.GetBytes(data));
        }
        public static string DecryptRsa(string encryptedText, string privateKey)
        {
            using (RSA rsa = RSA.Create())
            {
                var encryptedObject = Encoding.UTF8.GetBytes(encryptedText);
                rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);

                byte[] datosDescifrados = rsa.Decrypt(encryptedObject, RSAEncryptionPadding.OaepSHA256);
                var data = Encoding.UTF8.GetString(datosDescifrados);

                return data;
            }
        }
        public static string DecryptAes(string encryptedText, string password, int iterations = 600000)
        {
            byte[] combinedBytes = Convert.FromBase64String(encryptedText);

            // Extraer componentes
            byte[] salt = new byte[Constants.SALT_SIZE];
            byte[] iv = new byte[Constants.IV_SIZE];
            byte[] hmacHash = new byte[Constants.HMAC_SIZE];
            byte[] cipherText = new byte[combinedBytes.Length - Constants.SALT_SIZE - Constants.IV_SIZE - Constants.HMAC_SIZE];

            Buffer.BlockCopy(combinedBytes, 0, salt, 0, Constants.SALT_SIZE);
            Buffer.BlockCopy(combinedBytes, Constants.SALT_SIZE, iv, 0, Constants.IV_SIZE);
            Buffer.BlockCopy(combinedBytes, Constants.SALT_SIZE + Constants.IV_SIZE, hmacHash, 0, Constants.HMAC_SIZE);
            Buffer.BlockCopy(combinedBytes, Constants.SALT_SIZE + Constants.IV_SIZE + Constants.HMAC_SIZE, cipherText, 0, cipherText.Length);

            // Derivar clave
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA512);
            byte[] key = pbkdf2.GetBytes(Constants.KEY_SIZE);

            // Verificar HMAC
            using var hmac = new HMACSHA256(key);
            byte[] computedHmac = hmac.ComputeHash(cipherText);

            if (!computedHmac.SequenceEqual(hmacHash))
                throw new CryptographicException("HMAC no coincide - Posible manipulación de datos");

            // Descifrar
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;

            using var decryptor = aes.CreateDecryptor();
            byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}

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
        public static async Task<T> GetEncryptedObjectAsync<T>(byte[] encryptedObject, string privateKey) where T : GetLoginParam
        {
            return await Task.Run(() =>
            {
                using (RSA rsa = RSA.Create())
                {
                    // Importar la clave privada
                    rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);

                    // Descifrar el mensaje
                    byte[] datosDescifrados = rsa.Decrypt(encryptedObject, RSAEncryptionPadding.OaepSHA256);
                    var data = Encoding.UTF8.GetString(datosDescifrados);

                    return JsonSerializer.Deserialize<T>(data)!;
                }

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

    }
}

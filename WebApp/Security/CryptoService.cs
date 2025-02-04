namespace WebApp.Security
{
    using Common.Interfaces;
    using Microsoft.JSInterop;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    public class CryptoService : ICryptoService
    {
        private readonly IJSRuntime _jsRuntime;
        private const string ValidChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_+-=[]{}|;:,.<>?";

        public CryptoService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<string> EncryptRsaAsync(string publicKey, string data)
        {
            return await _jsRuntime.InvokeAsync<string>("encryptRsa", publicKey, data);
        }

        public async Task<string> EncryptAesAsync(string password, string data, int iterations = 600000)
        {
            return await _jsRuntime.InvokeAsync<string>("encryptAes", password, data, iterations);
        }

        public Task<string> DecryptRsaAsync(string privateKey, string data)
        {
            throw new NotImplementedException();
        }

        public Task<string> DecryptAesAsync(string password, string data, int iterations = 600000)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GenerateAesRandomKeyAsync(int size)
        {
            return await Task.Run(() =>
            {
                if (size <= 16)
                    throw new ArgumentException("La longitud de la contraseña debe ser al menos 16 caracteres.");

                using (var rng = RandomNumberGenerator.Create())
                {
                    var byteBuffer = new byte[size];
                    rng.GetBytes(byteBuffer);

                    var password = new StringBuilder(size);
                    foreach (var byteValue in byteBuffer)
                    {
                        password.Append(ValidChars[byteValue % ValidChars.Length]);
                    }

                    return password.ToString();
                }
            });
        }
    }
}

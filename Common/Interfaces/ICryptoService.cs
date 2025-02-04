using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface ICryptoService
    {
        Task<string> EncryptRsaAsync(string publicKey, string data);
        Task<string> DecryptRsaAsync(string privateKey, string data);
        Task<string> EncryptAesAsync(string password, string data, int iterations = 600000);
        Task<string> DecryptAesAsync(string password, string data, int iterations = 600000);
        Task<string> GenerateAesRandomKeyAsync(int size);
    }
}

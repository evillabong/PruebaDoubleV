using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public static class Constants
    {
        public const int KEY_SIZE = 32;          // AES-256
        public const int IV_SIZE = 16;           // Tamaño del IV para AES-CBC
        public const int SALT_SIZE = 32;         // Salt para PBKDF2
        public const int HMAC_SIZE = 32;         // HMAC-SHA256
    }
}

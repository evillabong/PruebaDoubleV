// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography;

Console.WriteLine("Hello, World!");
using (RSA rsa = RSA.Create(2048))
{
    // Obtener la clave privada en formato PEM
    string privateKey = ExportPrivateKey(rsa);

    // Obtener la clave pública en formato PEM
    string publicKey = ExportPublicKey(rsa);

    // Mostrar las claves generadas
    Console.WriteLine("Clave Privada:");
    Console.WriteLine(privateKey);
    Console.WriteLine();

    Console.WriteLine("Clave Pública:");
    Console.WriteLine(publicKey);
}
// Exportar la clave privada en formato PEM
static string ExportPrivateKey(RSA rsa)
{
    var privateKeyBytes = rsa.ExportRSAPrivateKey();
    return Convert.ToBase64String(privateKeyBytes, Base64FormattingOptions.None);
}

// Exportar la clave pública en formato PEM
static string ExportPublicKey(RSA rsa)
{
    var publicKeyBytes = rsa.ExportRSAPublicKey();
    return Convert.ToBase64String(publicKeyBytes, Base64FormattingOptions.None);
}
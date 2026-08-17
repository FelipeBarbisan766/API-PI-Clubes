// Infrastructure/Security/AesCpfEncryptionService.cs
using System.Security.Cryptography;
using System.Text;
using API_PI_Clubes.Infrastructure.Security.Interfaces;
using Microsoft.Extensions.Configuration;

namespace API_PI_Clubes.Infrastructure.Security
{
    public class AesCpfEncryptionService : ICpfEncryptionService
    {
        private readonly byte[] _encryptionKey;
        private readonly byte[] _hmacKey;
        private const int NonceSize = 12; // AES-GCM
        private const int TagSize = 16;

        public AesCpfEncryptionService(IConfiguration config)
        {
            var encKeyB64 = config["Security:CpfEncryptionKey"]
                ?? throw new InvalidOperationException("Security:CpfEncryptionKey não configurada");
            var hmacKeyB64 = config["Security:CpfHmacKey"]
                ?? throw new InvalidOperationException("Security:CpfHmacKey não configurada");

            _encryptionKey = Convert.FromBase64String(encKeyB64);
            _hmacKey = Convert.FromBase64String(hmacKeyB64);

            if (_encryptionKey.Length != 32)
                throw new InvalidOperationException("CpfEncryptionKey deve ter 32 bytes (256 bits) em Base64");
        }

        public string Encrypt(string cpf)
        {
            var plainBytes = Encoding.UTF8.GetBytes(cpf);

            var nonce = new byte[NonceSize];
            RandomNumberGenerator.Fill(nonce);

            var cipherBytes = new byte[plainBytes.Length];
            var tag = new byte[TagSize];

            using var aesGcm = new AesGcm(_encryptionKey, TagSize);
            aesGcm.Encrypt(nonce, plainBytes, cipherBytes, tag);

            var result = new byte[NonceSize + TagSize + cipherBytes.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
            Buffer.BlockCopy(tag, 0, result, NonceSize, TagSize);
            Buffer.BlockCopy(cipherBytes, 0, result, NonceSize + TagSize, cipherBytes.Length);

            return Convert.ToBase64String(result);
        }

        public string Decrypt(string encrypted)
        {
            var fullBytes = Convert.FromBase64String(encrypted);

            var nonce = fullBytes[..NonceSize];
            var tag = fullBytes[NonceSize..(NonceSize + TagSize)];
            var cipherBytes = fullBytes[(NonceSize + TagSize)..];

            var plainBytes = new byte[cipherBytes.Length];

            using var aesGcm = new AesGcm(_encryptionKey, TagSize);
            aesGcm.Decrypt(nonce, cipherBytes, tag, plainBytes);

            return Encoding.UTF8.GetString(plainBytes);
        }

        public string Hash(string cpf)
        {
            using var hmac = new HMACSHA256(_hmacKey);
            var bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(cpf));
            return Convert.ToHexString(bytes); 
        }
    }
}
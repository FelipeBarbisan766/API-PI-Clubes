namespace API_PI_Clubes.Infrastructure.Security.Interfaces
{
    public interface ICpfEncryptionService
    {
        string Encrypt(string cpf);
        string Decrypt(string encrypted);
        string Hash(string cpf);
    }
}
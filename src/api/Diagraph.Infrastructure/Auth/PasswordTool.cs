using System.Security.Cryptography;
using System.Text;
using Diagraph.Infrastructure.Hashing;

namespace Diagraph.Infrastructure.Auth;

public class PasswordTool
{
    private const char Separator = ':';
    private const int SaltBytes = 32; // TODO: configuration
    
    private readonly IHashTool _hashTool;

    public PasswordTool(IHashTool hashTool)
        => _hashTool = hashTool;
    
    public string Hash(string plainPassword)
    {
        byte[] salt = new byte[SaltBytes];
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);

        return Hash(plainPassword, salt);
    }
    
    public string Hash(string plainPassword, byte[] salt)
    {
        Span<byte> passwordBytes = Encoding.UTF8.GetBytes(plainPassword);

        Span<byte> passwordBuffer = stackalloc byte[SaltBytes + plainPassword.Length];
        salt.CopyTo(passwordBuffer);
        passwordBytes.CopyTo(passwordBuffer);

        string hash = _hashTool.ComputeHash(passwordBuffer.ToArray());
        return $"{Convert.ToBase64String(salt)}{Separator}{hash}";
    }

    public bool Compare(string passwordHash, string givenPassword)
    { 
        byte[] saltBytes = Convert.FromBase64String
        (
            passwordHash.Split(Separator).First()
        ); 

        return Hash(givenPassword, saltBytes).SequenceEqual(passwordHash);
    }
}
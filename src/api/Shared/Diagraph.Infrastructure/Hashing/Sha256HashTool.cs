using System.Security.Cryptography;
using System.Text;

namespace Diagraph.Infrastructure.Hashing;

public class Sha256HashTool : IHashTool
{
    public string ComputeHash(string input)
    {
        return ComputeHash(Encoding.UTF8.GetBytes(input));
    }

    public string ComputeHash(byte[] input)
    {
        using SHA256 sha256  = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(input);
        return Convert.ToBase64String(hashBytes);
    }
}
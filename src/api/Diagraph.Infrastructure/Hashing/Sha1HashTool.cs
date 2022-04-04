using System.Security.Cryptography;
using System.Text;

namespace Diagraph.Infrastructure.Hashing;

public class Sha1HashTool : IHashTool
{
    public string ComputeHash(string input)
    {
        return ComputeHash(Encoding.UTF8.GetBytes(input));
    }

    public string ComputeHash(byte[] input)
    {
        using SHA1 sha1  = SHA1.Create();
        byte[] hashBytes = sha1.ComputeHash(input);
        return Convert.ToBase64String(hashBytes);
    }
}
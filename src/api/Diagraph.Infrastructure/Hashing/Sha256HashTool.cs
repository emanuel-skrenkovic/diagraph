using System.Security.Cryptography;
using System.Text;

namespace Diagraph.Infrastructure.Hashing;

public class Sha256HashTool
{
    public string ComputeHash(string input)
    {
        using SHA256 sha1  = SHA256.Create();
        byte[] hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(hashBytes);
    }
}
using System.Security.Cryptography;
using System.Text;

namespace Diagraph.Infrastructure;

public class Sha1HashTool : IHashTool
{
    public string ComputeHash(string input)
    {
        using SHA1 sha1  = SHA1.Create();
        byte[] hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(hashBytes);
    }
}
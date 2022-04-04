namespace Diagraph.Infrastructure.Hashing;

public interface IHashTool
{
    string ComputeHash(string input);

    string ComputeHash(byte[] input);
}
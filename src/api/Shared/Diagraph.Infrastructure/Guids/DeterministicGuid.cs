using System.Security.Cryptography;
using System.Text;

namespace Diagraph.Infrastructure.Guids;

public static class DeterministicGuid
{
    /// <summary>
    /// RFC 4122 (name-based uuids):
    ///        
    /// * The requirements for these types of UUIDs are as follows:
    ///
    /// o  The UUIDs generated at different times from the same name in the
    ///     same namespace MUST be equal.
    ///
    /// o  The UUIDs generated from two different names in the same namespace
    ///    should be different (with very high probability).
    ///
    /// o  The UUIDs generated from the same name in two different namespaces
    ///    should be different with (very high probability).
    ///
    /// o  If two UUIDs that were generated from names are equal, then they
    ///    were generated from the same name in the same namespace (with very
    ///    high probability).
    ///
    /// The algorithm for generating a UUID from a name and a name space are
    ///     as follows:
    ///
    /// o  Allocate a UUID to use as a "name space ID" for all UUIDs
    /// generated from names in that name space; see Appendix C for some
    /// pre-defined values.
    /// o  Choose either MD5 [4] or SHA-1 [8] as the hash algorithm; If
    ///     backward compatibility is not an issue, SHA-1 is preferred.
    /// </summary>
    public static Guid New(string @namespace, string name)
    {
        using var sha1 = SHA1.Create();
        
        byte[]     bytes = Encoding.UTF8.GetBytes(@namespace + name);
        Span<byte> hash  = sha1.ComputeHash(bytes).AsSpan();
        
        Span<byte> resultBytesSpan = new byte[16].AsSpan();

        hash[..16].CopyTo(resultBytesSpan);
        
        //set high-nibble to 5 to indicate type 5
        resultBytesSpan[6] &= 0x0F; 
        resultBytesSpan[6] |= 0x50; 

        //set upper two bits to "10"
        resultBytesSpan[8] &= 0x3F; 
        resultBytesSpan[8] |= 0x80; 
            
        return new Guid(resultBytesSpan);
    }
}
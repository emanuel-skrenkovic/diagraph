using System.Text;
using Microsoft.AspNetCore.Http;

namespace Diagraph.Core.Extensions;

public static class FormFileExtensions
{
    public static async Task<string> ReadAsync(this IFormFile formFile)
    {
         MemoryStream dataStream = new MemoryStream();
         await formFile.CopyToAsync(dataStream);
 
         return Encoding.UTF8.GetString(dataStream.ToArray());       
    }
}
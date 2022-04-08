namespace Diagraph.Infrastructure.Email;

public class EmailServerConfiguration
{
    public IEnumerable<string> From { get; set; }
    
    public string Host { get; set; }
    
    public int Port { get; set; }
}
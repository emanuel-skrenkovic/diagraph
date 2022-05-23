namespace Diagraph.Modules.Identity;

public enum ExternalProvider
{
    Google
}

public class External
{
    public int Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public ExternalProvider Provider { get; set; }
    
    public string Data { get; set; }
}
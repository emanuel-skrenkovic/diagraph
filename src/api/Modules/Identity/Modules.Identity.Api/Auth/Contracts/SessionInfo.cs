namespace Diagraph.Modules.Identity.Api.Auth.Contracts;

public class SessionInfo
{
    public string UserName { get; set; }
    
    public bool IsAuthenticated { get; set; }
    
    public string AuthenticationType { get; set; }
}
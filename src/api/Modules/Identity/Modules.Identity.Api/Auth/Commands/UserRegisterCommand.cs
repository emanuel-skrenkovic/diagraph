namespace Diagraph.Modules.Identity.Api.Auth.Commands;

public class UserRegisterCommand
{
    public string UserName { get; set; }
    
    public string Email { get; set; }
    
    public string Password { get; set; }
}
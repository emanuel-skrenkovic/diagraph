namespace Diagraph.Modules.Identity.Api.Auth.Commands;

public class LoginCommand
{
    public string Email { get; set; }
    
    public string Password { get; set; }
}
namespace Diagraph.Api.Users.Models;

public class LoginRequest
{
    public string Email { get; set; }
    
    public string Password { get; set; }
}
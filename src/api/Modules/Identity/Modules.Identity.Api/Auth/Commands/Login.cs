using System.ComponentModel.DataAnnotations;

namespace Diagraph.Modules.Identity.Api.Auth.Commands;

public class LoginResult
{
    public string Reason { get; set; }
}

public class LoginCommand
{
    [Required] public string Email { get; set; }
    
    [Required] public string Password { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace Diagraph.Modules.Identity.Api.Auth.Contracts;

public class LoginCommand
{
    [Required] public string Email { get; set; }
    
    [Required] public string Password { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace Diagraph.Modules.Identity.Api.Auth.Commands;

public class UserRegisterCommand
{
    [Required]
    public string Email { get; set; }
    
    [Required]
    public string Password { get; set; }
}
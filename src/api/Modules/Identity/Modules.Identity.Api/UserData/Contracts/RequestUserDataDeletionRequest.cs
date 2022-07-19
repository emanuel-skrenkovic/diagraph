using System.ComponentModel.DataAnnotations;

namespace Diagraph.Modules.Identity.Api.UserData.Contracts;

public class RequestUserDataDeletionRequest
{
    [Required] public string Email { get; set; }
}
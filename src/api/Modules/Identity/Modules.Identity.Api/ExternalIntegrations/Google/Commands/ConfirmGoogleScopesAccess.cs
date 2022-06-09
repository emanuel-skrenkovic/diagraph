using System.ComponentModel.DataAnnotations;
using Diagraph.Infrastructure.Api;

namespace Diagraph.Modules.Identity.Api.ExternalIntegrations.Google.Commands;

public class ConfirmGoogleTasksScopesCommand : IIdempotentRequest
{
    [Required] public string Code { get; set; }
    
    public string RedirectUri { get; set; }
    
    public string[] Scope { get; set; }
    
    [Required] public string IdempotencyKey { get; set; }
}
namespace Diagraph.Modules.Identity.Api.ExternalIntegrations.Google.Commands;

public class ConfirmGoogleTasksScopesCommand
{
    public string Code { get; set; }
    
    public string RedirectUri { get; set; }
    
    public string[] Scope { get; set; }
}
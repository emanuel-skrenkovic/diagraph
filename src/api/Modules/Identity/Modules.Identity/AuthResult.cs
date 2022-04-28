namespace Diagraph.Modules.Identity;

public record AuthResult(bool Authenticated, string Reason = null);
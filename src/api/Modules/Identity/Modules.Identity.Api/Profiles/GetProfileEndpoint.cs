using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Modules.Identity.Database;
using FastEndpoints;

namespace Diagraph.Modules.Identity.Api.Profiles;

public class GetProfileEndpoint : EndpointWithoutRequest
{
    private readonly IdentityDbContext _context;
    private readonly IUserContext      _userContext;

    public GetProfileEndpoint(IdentityDbContext context, IUserContext userContext)
    {
        _context     = context;
        _userContext = userContext;
    }
    
    public override void Configure() => Get("my/profile");

    public override async Task HandleAsync(CancellationToken ct)
    {
        UserProfile profile = await _context.GetOrAddAsync
        (
            p => p.UserId == _userContext.UserId, 
            UserProfile.Create(_userContext.UserId)
        );

        await _context.SaveChangesAsync(ct);

        // TODO: might be incorrectly serialized json.
        await SendOkAsync(profile.Data, ct);
    }
}
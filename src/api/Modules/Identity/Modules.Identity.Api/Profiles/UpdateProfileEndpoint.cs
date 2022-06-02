using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Database.Extensions;
using Diagraph.Modules.Identity.Database;
using FastEndpoints;

namespace Diagraph.Modules.Identity.Api.Profiles;

public class UpdateProfileEndpoint : Endpoint<PlainTextRequest>
{
    private readonly IdentityDbContext _context;
    private readonly IUserContext      _userContext;

    public UpdateProfileEndpoint(IdentityDbContext context, IUserContext userContext)
    {
        _context     = context;
        _userContext = userContext;
    }

    public override void Configure() => Put("my/profile");

    public override async Task HandleAsync(PlainTextRequest req, CancellationToken ct)
    {
        UserProfile profile = await _context.GetOrAddAsync
        (
            p => p.UserId == _userContext.UserId,
            UserProfile.Create(_userContext.UserId)
        );

        profile.Data = req.Content;

        _context.Update(profile);
        await _context.SaveChangesAsync(ct);

        await SendOkAsync(ct);
    }
}
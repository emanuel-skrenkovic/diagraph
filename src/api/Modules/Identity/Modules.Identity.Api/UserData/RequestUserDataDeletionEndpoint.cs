using System.Net;
using Diagraph.Infrastructure.Api.Extensions;
using Diagraph.Infrastructure.ProcessManager.Contracts;
using Diagraph.Modules.Identity.Api.UserData.Contracts;
using Diagraph.Modules.Identity.Database;
using Diagraph.Modules.Identity.UserData;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Diagraph.Modules.Identity.Api.UserData;

public class RequestUserDataDeletionEndpoint : Endpoint<RequestUserDataDeletionRequest>
{
    private readonly IdentityDbContext                     _context;
    private readonly IProcessManager<UserDataRemovalState> _deleteProcessManager;

    public RequestUserDataDeletionEndpoint
    (
        IdentityDbContext                     context,
        IProcessManager<UserDataRemovalState> deleteProcessManager
    )
    {
        _context              = context;
        _deleteProcessManager = deleteProcessManager;
    }

    public override void Configure() => Put("/TODO/");

    public override async Task HandleAsync(RequestUserDataDeletionRequest req, CancellationToken ct)
    {
        User user = await _context.Users.SingleOrDefaultAsync(u => u.Email == req.Email, ct);
        if (user is null)
        {
            await SendErrorsAsync(cancellation: ct);
            return;
        }

        UserDataRemovalState removalProcessState = UserDataRemovalState.Create(req.Email);
        await _deleteProcessManager.ScheduleAsync(removalProcessState);

        await this.SendWithoutContentAsync((int)HttpStatusCode.Accepted, ct);
    }
}
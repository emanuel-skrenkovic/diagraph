using Diagraph.Infrastructure.ErrorHandling;
using Diagraph.Modules.Identity.Registration;
using FastEndpoints;

namespace Diagraph.Modules.Identity.Api.Auth;

public class RegisterConfirmEndpoint : EndpointWithoutRequest
{
    private readonly UserConfirmation _userConfirmation;

    public RegisterConfirmEndpoint(UserConfirmation userConfirmation)
        => _userConfirmation = userConfirmation;

    public override void Configure()
    {
        Get("register/confirm");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string token              = Query<string>("token");
        Result confirmationResult = await _userConfirmation.ValidateUserConfirmationAsync(token);

        await confirmationResult.Match
        (
            () => SendOkAsync(ct),
            _  => SendAsync("Failed to validate confirmation token.", 400, ct)
        );
    }
}
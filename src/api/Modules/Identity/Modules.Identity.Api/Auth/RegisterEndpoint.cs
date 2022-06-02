using Diagraph.Infrastructure.Emails;
using Diagraph.Modules.Identity.Api.Auth.Commands;
using Diagraph.Modules.Identity.Database;
using Diagraph.Modules.Identity.Registration;
using Diagraph.Modules.Identity.ValueObjects;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using DiagraphUser = Diagraph.Modules.Identity.User;

namespace Diagraph.Modules.Identity.Api.Auth;

public class RegisterEndpoint : Endpoint<UserRegisterCommand>
{
    private readonly IdentityDbContext _context;
    private readonly PasswordTool      _passwordTool;
    private readonly UserConfirmation  _userConfirmation;

    public RegisterEndpoint
    (
        IdentityDbContext context,
        PasswordTool      passwordTool,
        UserConfirmation  userConfirmation
    )
    {
        _context          = context;
        _passwordTool     = passwordTool;
        _userConfirmation = userConfirmation;
    }

    public override void Configure()
    {
        Post("auth/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UserRegisterCommand req, CancellationToken ct)
    {
        if (await _context.Users.AnyAsync(u => u.Email == req.Email, ct))
        {
            // Just return ok if the user already exists. If it's a valid request,
            // the user will check their email.
            await SendOkAsync(ct);
            return;
        }

        User user = DiagraphUser.Create
        (
            EmailAddress.Create(req.Email),
            Password.Create(req.Password),
            _passwordTool
        );
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync(ct);
        
        // TODO: read from database and send on timer, akin to outbox?
        HttpRequest request = HttpContext.Request;
        await _userConfirmation.SendConfirmationEmailAsync
        (
            user, 
            new Uri($"{request.Scheme}://{request.Host}")
        );
        
        await SendOkAsync(ct);
    }
}
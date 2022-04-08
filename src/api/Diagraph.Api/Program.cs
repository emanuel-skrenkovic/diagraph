using System.Security.Claims;
using Diagraph.Api;
using Diagraph.Infrastructure;
using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Database;
using Diagraph.Infrastructure.Email;
using Diagraph.Infrastructure.Hashing;
using Diagraph.Infrastructure.Parsing;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IGlucoseDataParser, LibreViewCsvGlucoseDataParser>();
builder.Services.AddScoped<IHashTool, Sha1HashTool>();
builder.Services.AddScoped<GlucoseDataImport>();
builder.Services.AddScoped<PasswordTool>();
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddAutoMapper(typeof(DiagraphDbContext).Assembly);

builder.Services.AddSingleton(new EmailServerConfiguration
{
    From = builder.Configuration.GetValue<string[]>("Mailhog:From"),
    Host = builder.Configuration["Mailhog:Host"],
    Port = builder.Configuration.GetValue<int>("Mailhog:Port")
});
builder.Services.AddScoped<EmailClient>();

// Add services to the container.
builder.Services.AddDbContext<DiagraphDbContext>
(
    options => options.UseNpgsql
    (
        builder.Configuration["Postgres:ConnectionString"],
        b => b.MigrationsAssembly("Diagraph.Infrastructure")
    )
);

builder.Services.AddAuthentication
(
    opts =>
    {
        opts.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opts.DefaultChallengeScheme    = CookieAuthenticationDefaults.AuthenticationScheme;
        opts.DefaultScheme             = CookieAuthenticationDefaults.AuthenticationScheme;
        opts.DefaultSignOutScheme      = CookieAuthenticationDefaults.AuthenticationScheme;
    }
).AddCookie
(
    opts =>
    {
        opts.Cookie.HttpOnly    = true;
        opts.Cookie.IsEssential = true;
        opts.Cookie.SameSite    = SameSiteMode.None;
        
        opts.LoginPath       = "/auth/login";
        opts.LogoutPath      = "/auth/logout";
    }
);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(opts =>
{
    opts.WithOrigins("http://localhost:3000");
    opts.AllowAnyHeader();
    opts.AllowAnyMethod();
    opts.Build();
    opts.AllowCredentials();
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Needs to be after UserAuthentication.
app.Use((context, next) =>
{
    if (context.User.Identity?.IsAuthenticated == true)
    {
        // lol
        UserContext userContext = (UserContext) context
            .RequestServices
            .GetRequiredService<IUserContext>();
        
        userContext.UserId = Guid.Parse
        (
            context.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value
        );
    }
    
    return next();
});

app.MapControllers();

app.Run();
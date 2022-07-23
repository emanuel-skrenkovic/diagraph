using Diagraph.Infrastructure.Api;
using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Events.EventStore;
using Diagraph.Infrastructure.Modules.Extensions;
using Diagraph.Modules.Events.Api;
using Diagraph.Modules.GlucoseData.Api;
using Diagraph.Modules.Identity.Api;
using FastEndpoints;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

// TODO: rename project into Diagraph.Host

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables(prefix: "DIAGRAPH_");

// TODO: remove AddControllers() call. Currently, when not present
// the AddProblemDetails() call fails.
ApplicationPartManager partManager = builder
    .Services
    .AddControllers()
    .PartManager;
partManager.ApplicationParts.Clear();
builder.Services.AddFastEndpoints();

string env = builder.Environment.EnvironmentName;
builder.Services.LoadModule<IdentityModule>(env);
builder.Services.LoadModule<GlucoseDataModule>(env);
builder.Services.LoadModule<EventsModule>(env);
builder.Services.LoadModule<EventStoreModule>
(
    // TODO: helper
    configuration: new ConfigurationManager()
        .AddJsonFile($"appsettings.{env}.json", optional: true)
        .Build()
);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession
(
    opts =>
    {
        opts.Cookie.HttpOnly     = true;
        opts.Cookie.IsEssential  = true;
        opts.Cookie.SameSite     = SameSiteMode.None;
        opts.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    }
);

builder.Services.AddProblemDetails(opts =>
{
    opts.MapToStatusCode<Exception>(400); // TODO: It's not me, it's you.
});

builder.Services.AddScoped<IUserContext, UserContext>();


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
    opts.WithHeaders("content-type");
    opts.WithExposedHeaders("location");
    opts.AllowAnyMethod();
    opts.AllowCredentials();
    opts.Build();
});

app.UseProblemDetails();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

// Needs to be after UserAuthentication.
app.Use(CorrelationIdMiddleware.Handle);
app.Use(UserContextMiddleware.Handle);

app.UseFastEndpoints();

app.Run();

using Diagraph.Infrastructure.Auth;
using Diagraph.Infrastructure.Modules.Extensions;
using Diagraph.Modules.Events.Api;
using Diagraph.Modules.GlucoseData.Api;
using Diagraph.Modules.Identity.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables(prefix: "DIAGRAPH_");

string env = builder.Environment.EnvironmentName;
builder.Services.LoadModule<IdentityModule>(env);
builder.Services.LoadModule<GlucoseDataModule>(env);
builder.Services.LoadModule<EventsModule>(env);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opts =>
{
    opts.Cookie.HttpOnly     = true;
    opts.Cookie.IsEssential  = true;
    opts.Cookie.SameSite     = SameSiteMode.None;
    opts.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddScoped<IUserContext, UserContext>();

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
    opts.WithHeaders("content-type");
    opts.WithExposedHeaders("location");
    opts.AllowAnyMethod();
    opts.AllowCredentials();
    opts.Build();
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// Needs to be after UserAuthentication.
app.Use(UserContextMiddleware.Handle);

app.MapControllers();

app.Run();
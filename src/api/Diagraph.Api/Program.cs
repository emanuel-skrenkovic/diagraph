using Diagraph.Infrastructure;
using Diagraph.Infrastructure.Database;
using Diagraph.Infrastructure.Parsing;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IGlucoseDataParser, LibreViewCsvGlucoseDataParser>();
builder.Services.AddScoped<IHashTool, Sha1HashTool>();
builder.Services.AddScoped<GlucoseDataImport>();
builder.Services.AddAutoMapper(typeof(DiagraphDbContext).Assembly);

// Add services to the container.
builder.Services.AddDbContext<DiagraphDbContext>
(
    options => options.UseNpgsql
    (
        builder.Configuration["Postgres:ConnectionString"],
        b => b.MigrationsAssembly("Diagraph.Infrastructure")
    )
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
    opts.AllowAnyOrigin();
    opts.AllowAnyHeader();
    opts.AllowAnyMethod();
    opts.Build();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
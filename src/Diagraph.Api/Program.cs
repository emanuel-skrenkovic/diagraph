using Diagraph.Infrastructure.Database;
using Diagraph.Infrastructure.Parsing;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IGlucoseDataParser, LibreViewCsvGlucoseDataParser>();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
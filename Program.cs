using AmericanAirlinesApi.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        opts.JsonSerializerOptions.WriteIndented = true;
    });

// SQLite - gera arquivo AmericanAirlines.db na raiz do projeto
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlite("Data Source=AmericanAirlines.db"));

// OpenAPI nativo .NET 10
builder.Services.AddOpenApi();

var app = builder.Build();

// Aplica migrations automaticamente ao iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Documenta a API — acesse em: http://localhost:5000/scalar/v1
app.MapOpenApi();
app.MapScalarApiReference();

app.UseAuthorization();
app.MapControllers();

Console.WriteLine("✈️  AmericanAirlines SkyAPI rodando!");
Console.WriteLine("📄  Docs: http://localhost:5000/scalar/v1");

app.Run();

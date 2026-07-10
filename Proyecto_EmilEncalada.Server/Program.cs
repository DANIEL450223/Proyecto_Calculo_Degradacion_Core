using Microsoft.EntityFrameworkCore;
using Proyecto_EmilEncalada.Server.Data;
using System.Text.Json.Serialization;
using Proyecto_EmilEncalada.Server.Interfaces;
using Proyecto_EmilEncalada.Server.Repositories;
using Proyecto_EmilEncalada.Server.Strategies;
using Proyecto_EmilEncalada.Server.Services;

var builder = WebApplication.CreateBuilder(args);

var frontendOrigins = builder.Configuration["FrontendOrigins"]?
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend", policy =>
    {
        var allowedOrigins = new[]
        {
            "http://localhost:5173",
            "http://localhost:5174",
            "http://localhost:5175",
            "http://localhost:5176",
            "http://localhost:5177",
            "http://localhost:5178",
            "https://localhost:5173",
            "https://localhost:5174",
            "https://localhost:5175",
            "https://localhost:5176",
            "https://localhost:5177",
            "https://localhost:5178",
            "https://proyecto-calculo-degradacion-core.vercel.app",
            "https://api-core.vercel.app"
        }.Concat(frontendOrigins).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();

        policy
            .SetIsOriginAllowed(origin =>
            {
                if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                {
                    return false;
                }

                return allowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase)
                    || uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase)
                    || uri.Host.EndsWith(".vercel.app", StringComparison.OrdinalIgnoreCase);
            })
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IEquipoRepository, EquipoRepository>();
builder.Services.AddScoped<ICalculoDegradacionStrategy, CalculoDegradacionPorReglasStrategy>();
builder.Services.AddScoped<DegradacionService>();

var app = builder.Build();

await DatabaseBootstrapper.EnsureReadyAsync(app.Services);

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors("PermitirFrontend");

app.UseSwagger();
app.UseSwaggerUI();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();

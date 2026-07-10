using Microsoft.EntityFrameworkCore;
using Proyecto_EmilEncalada.Server.Data;
using System.Text.Json.Serialization;
using Proyecto_EmilEncalada.Server.Interfaces;
using Proyecto_EmilEncalada.Server.Repositories;
using Proyecto_EmilEncalada.Server.Strategies;
using Proyecto_EmilEncalada.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend", policy =>
    {
        policy
            .WithOrigins(
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
                "https://proyecto-calculo-degradacion-core.vercel.app",

                "https://api-core.vercel.app"
            )
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
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IEquipoRepository, EquipoRepository>();
builder.Services.AddScoped<ICalculoDegradacionStrategy, CalculoDegradacionPorReglasStrategy>();
builder.Services.AddScoped<DegradacionService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors("PermitirFrontend");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
using Microsoft.AspNetCore.Mvc;
using Proyecto_EmilEncalada.Server.Services;

namespace Proyecto_EmilEncalada.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CoreController : ControllerBase
{
    private readonly DegradacionService _degradacionService;

    public CoreController(DegradacionService degradacionService)
    {
        _degradacionService = degradacionService;
    }

    [HttpGet("degradacion/{id}")]
    public async Task<IActionResult> CalcularDegradacion(int id)
    {
        var resultado = await _degradacionService.CalcularDegradacionAsync(id);

        if (resultado == null)
        {
            return NotFound(new
            {
                mensaje = "No se encontró el equipo solicitado."
            });
        }

        return Ok(resultado);
    }
}
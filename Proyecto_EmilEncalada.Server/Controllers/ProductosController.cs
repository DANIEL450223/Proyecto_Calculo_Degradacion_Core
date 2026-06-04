using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_EmilEncalada.Server.Data;
using Proyecto_EmilEncalada.Server.Models;
using Proyecto_EmilEncalada.Server.Helpers;

namespace Proyecto_EmilEncalada.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
                return NotFound(new { mensaje = "Producto no encontrado." });

            return producto;
        }

        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            if (!SeguridadHelper.EsAdmin(Request))
                return SeguridadHelper.AccesoDenegado();

            if (string.IsNullOrWhiteSpace(producto.Nombre))
                return BadRequest(new { mensaje = "El nombre del producto es obligatorio." });

            if (producto.Precio <= 0)
                return BadRequest(new { mensaje = "El precio debe ser mayor a cero." });

            if (producto.Stock < 0)
                return BadRequest(new { mensaje = "El stock no puede ser negativo." });

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, producto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, Producto producto)
        {
            if (!SeguridadHelper.EsAdmin(Request))
                return SeguridadHelper.AccesoDenegado();

            if (id != producto.Id)
                return BadRequest(new { mensaje = "El id no coincide." });

            if (string.IsNullOrWhiteSpace(producto.Nombre))
                return BadRequest(new { mensaje = "El nombre del producto es obligatorio." });

            if (producto.Precio <= 0)
                return BadRequest(new { mensaje = "El precio debe ser mayor a cero." });

            if (producto.Stock < 0)
                return BadRequest(new { mensaje = "El stock no puede ser negativo." });

            var existeProducto = await _context.Productos.AnyAsync(p => p.Id == id);

            if (!existeProducto)
                return NotFound(new { mensaje = "Producto no encontrado." });

            _context.Entry(producto).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            if (!SeguridadHelper.EsAdmin(Request))
                return SeguridadHelper.AccesoDenegado();

            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
                return NotFound(new { mensaje = "Producto no encontrado." });

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
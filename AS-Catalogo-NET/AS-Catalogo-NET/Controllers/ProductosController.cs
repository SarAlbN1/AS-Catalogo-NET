using Catalogo.Api.DTOs;
using Catalogo.Api.Models;
using Catalogo.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Catalogo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController(IProductosService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Producto>>> GetAll(
        [FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => Ok(await service.ListarAsync(q, page, pageSize));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Producto>> GetById(int id)
    {
        var p = await service.ObtenerAsync(id);
        return p is null ? NotFound() : Ok(p);
    }

    [HttpPost]
    public async Task<ActionResult<Producto>> Create([FromBody] ProductoCreateDto dto)
    {
        var creado = await service.CrearAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Producto>> Update(int id, [FromBody] ProductoUpdateDto dto)
    {
        var actualizado = await service.ActualizarAsync(id, dto);
        return actualizado is null ? NotFound() : Ok(actualizado);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => await service.EliminarAsync(id) ? NoContent() : NotFound();
}

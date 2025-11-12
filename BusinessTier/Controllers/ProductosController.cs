using BusinessTier.DTOs;

using BusinessTier.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusinessTier.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly IProductosService _svc;
    public ProductosController(IProductosService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductoDto>>> GetAll() =>
        Ok(await _svc.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductoDto>> GetById(int id)
    {
        var p = await _svc.GetByIdAsync(id);
        return p is null ? NotFound() : Ok(p);
    }

    [HttpPost]
    public async Task<ActionResult<ProductoDto>> Create([FromBody] ProductoCreateDto req)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var p = await _svc.CreateAsync(req);
        return CreatedAtAction(nameof(GetById), new { id = p.Id }, p);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductoDto>> Update(int id, [FromBody] ProductoUpdateDto req)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var p = await _svc.UpdateAsync(id, req);
        return p is null ? NotFound() : Ok(p);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        await _svc.DeleteAsync(id) ? NoContent() : NotFound();
}

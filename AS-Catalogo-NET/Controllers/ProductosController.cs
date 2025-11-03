using AS_Catalogo_NET.Model;
using AS_Catalogo_NET.Services;
using Microsoft.AspNetCore.Mvc;

namespace AS_Catalogo_NET.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly IProductosService _svc;

    public ProductosController(IProductosService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Producto>>> GetAll()
        => Ok(await _svc.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Producto>> GetById(int id)
    {
        var p = await _svc.GetByIdAsync(id);
        return p is null ? NotFound() : Ok(p);
    }

    [HttpPost]
    public async Task<ActionResult<Producto>> Create([FromBody] Producto p)
    {
        var created = await _svc.CreateAsync(p);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Producto>> Update(int id, [FromBody] Producto p)
    {
        var updated = await _svc.UpdateAsync(id, p);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => await _svc.DeleteAsync(id) ? NoContent() : NotFound();
}

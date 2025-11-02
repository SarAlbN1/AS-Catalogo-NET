using Catalogo.Api.Data;
using Catalogo.Api.DTOs;
using Catalogo.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.Api.Services;

public class ProductosService : IProductosService
{
    private readonly CatalogoDb _db;

    public ProductosService(CatalogoDb db) => _db = db;

    public async Task<IEnumerable<Producto>> ListarAsync(string? q = null, int page = 1, int pageSize = 20)
    {
        var query = _db.Productos.AsNoTracking().OrderBy(p => p.Id).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim();
            query = query.Where(p => p.Nombre.Contains(q));
        }
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 20 : Math.Min(pageSize, 100);

        return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public Task<Producto?> ObtenerAsync(int id) =>
        _db.Productos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Producto> CrearAsync(ProductoCreateDto dto)
    {
        var entity = new Producto
        {
            Nombre = dto.Nombre.Trim(),
            Precio = dto.Precio,
            Stock  = dto.Stock,
            Activo = true
        };
        _db.Productos.Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task<Producto?> ActualizarAsync(int id, ProductoUpdateDto dto)
    {
        var entity = await _db.Productos.FirstOrDefaultAsync(p => p.Id == id);
        if (entity is null) return null;

        entity.Nombre = dto.Nombre.Trim();
        entity.Precio = dto.Precio;
        entity.Stock  = dto.Stock;
        entity.Activo = dto.Activo;

        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var entity = await _db.Productos.FirstOrDefaultAsync(p => p.Id == id);
        if (entity is null) return false;

        _db.Productos.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}

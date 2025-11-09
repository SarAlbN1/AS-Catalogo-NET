using AS_Catalogo_NET.DTOs;
using AS_Catalogo_NET.Model;
using AS_Catalogo_NET.Persistance;
using Microsoft.EntityFrameworkCore;

namespace AS_Catalogo_NET.Gateways;

public class EfProductosGateway : IProductosGateway
{
    private readonly MyAppDbContext _db;
    public EfProductosGateway(MyAppDbContext db) => _db = db;

    public async Task<IEnumerable<ProductoDto>> GetAllAsync() =>
        (await _db.Productos.AsNoTracking().ToListAsync()).Select(Map);

    public async Task<ProductoDto?> GetByIdAsync(int id)
    {
        var p = await _db.Productos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return p is null ? null : Map(p);
    }

    public async Task<ProductoDto> CreateAsync(ProductoCreateDto dto)
    {
        var e = new Producto {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion ?? "",
            Precio = dto.Precio,
            CantidadDisponible = dto.CantidadDisponible,
            FechaCreacion = DateTime.UtcNow,
            CatalogoId = dto.CatalogoId
        };
        _db.Productos.Add(e);
        await _db.SaveChangesAsync();
        return Map(e);
    }

    public async Task<ProductoDto> UpdateAsync(int id, ProductoUpdateDto dto)
    {
        var e = await _db.Productos.FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new KeyNotFoundException();
        e.Nombre = dto.Nombre;
        e.Descripcion = dto.Descripcion ?? "";
        e.Precio = dto.Precio;
        e.CantidadDisponible = dto.CantidadDisponible;
        await _db.SaveChangesAsync();
        return Map(e);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var e = await _db.Productos.FirstOrDefaultAsync(x => x.Id == id);
        if (e is null) return false;
        _db.Productos.Remove(e);
        await _db.SaveChangesAsync();
        return true;
    }

    private static ProductoDto Map(Producto p) => new()
    {
        Id = p.Id,
        Nombre = p.Nombre,
        Descripcion = p.Descripcion,
        Precio = p.Precio,
        CantidadDisponible = p.CantidadDisponible,
        FechaCreacion = p.FechaCreacion,
        CatalogoId = p.CatalogoId
    };
}

using AS_Catalogo_NET.Model;
using AS_Catalogo_NET.Persistance;
using Microsoft.EntityFrameworkCore;

namespace AS_Catalogo_NET.Services;

public class ProductosService : IProductosService
{
    private readonly MyAppDbContext _db;

    public ProductosService(MyAppDbContext db) => _db = db;

    public async Task<List<Producto>> GetAllAsync()
        => await _db.Productos.AsNoTracking().ToListAsync();

    public async Task<Producto?> GetByIdAsync(int id)
        => await _db.Productos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Producto> CreateAsync(Producto p)
    {
        if (p.CatalogoId == 0) p.CatalogoId = 1;
        _db.Productos.Add(p);
        await _db.SaveChangesAsync();
        return p;
    }

    public async Task<Producto?> UpdateAsync(int id, Producto p)
    {
        var dbp = await _db.Productos.FindAsync(id);
        if (dbp is null) return null;

        dbp.Nombre = p.Nombre;
        dbp.Descripcion = p.Descripcion;
        dbp.Precio = p.Precio;
        dbp.CantidadDisponible = p.CantidadDisponible;
        dbp.CatalogoId = p.CatalogoId == 0 ? dbp.CatalogoId : p.CatalogoId;

        await _db.SaveChangesAsync();
        return dbp;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var dbp = await _db.Productos.FindAsync(id);
        if (dbp is null) return false;

        _db.Productos.Remove(dbp);
        await _db.SaveChangesAsync();
        return true;
    }
}

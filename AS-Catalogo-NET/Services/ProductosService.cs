using AS_Catalogo_NET.DTOs;
using AS_Catalogo_NET.GrpcClients;
using AS_Catalogo_NET.Model;
using AS_Catalogo_NET.Persistance;
using Microsoft.EntityFrameworkCore;

namespace AS_Catalogo_NET.Services;

public class ProductosService : IProductosService
{
    private readonly MyAppDbContext _db;
    private readonly ProductosGrpcClient _grpc;
    private readonly bool _preferGrpc;

    public ProductosService(MyAppDbContext db, ProductosGrpcClient grpc, IConfiguration cfg)
    {
        _db = db;
        _grpc = grpc;
        _preferGrpc = cfg.GetValue<bool>("PreferGrpc");
    }

    // ==== READ ALL ====
    public async Task<IEnumerable<ProductoDto>> GetAllAsync()
    {
        if (_preferGrpc) return await _grpc.GetAllAsync();

        var list = await _db.Productos.AsNoTracking().ToListAsync();
        return list.Select(Map);
    }

    // ==== READ BY ID ====
    public async Task<ProductoDto?> GetByIdAsync(int id)
    {
        if (_preferGrpc) return await _grpc.GetByIdAsync(id);

        var p = await _db.Productos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return p is null ? null : Map(p);
    }

    // ==== CREATE ====
    public async Task<ProductoDto> CreateAsync(ProductoCreateDto dto)
    {
        if (_preferGrpc) return await _grpc.CreateAsync(dto);

        var entity = new Producto
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion ?? "",
            Precio = dto.Precio,
            CantidadDisponible = dto.CantidadDisponible,
            FechaCreacion = DateTime.UtcNow,
            CatalogoId = dto.CatalogoId
        };
        _db.Productos.Add(entity);
        await _db.SaveChangesAsync();
        return Map(entity);
    }

    // ==== UPDATE ====
    public async Task<ProductoDto?> UpdateAsync(int id, ProductoUpdateDto dto)
    {
        if (_preferGrpc) return await _grpc.UpdateAsync(id, dto);

        var entity = await _db.Productos.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return null;

        entity.Nombre = dto.Nombre;
        entity.Descripcion = dto.Descripcion ?? "";
        entity.Precio = dto.Precio;
        entity.CantidadDisponible = dto.CantidadDisponible;

        await _db.SaveChangesAsync();
        return Map(entity);
    }

    // ==== DELETE ====
    public async Task<bool> DeleteAsync(int id)
    {
        if (_preferGrpc) return await _grpc.DeleteAsync(id);

        var entity = await _db.Productos.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return false;

        _db.Productos.Remove(entity);
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

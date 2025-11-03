using AS_Catalogo_NET.Model;

namespace AS_Catalogo_NET.Services;

public interface IProductosService
{
    Task<List<Producto>> GetAllAsync();
    Task<Producto?> GetByIdAsync(int id);
    Task<Producto> CreateAsync(Producto p);
    Task<Producto?> UpdateAsync(int id, Producto p);
    Task<bool> DeleteAsync(int id);
}

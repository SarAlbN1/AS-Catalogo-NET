using Catalogo.Api.DTOs;
using Catalogo.Api.Models;

namespace Catalogo.Api.Services;

public interface IProductosService
{
    Task<IEnumerable<Producto>> ListarAsync(string? q = null, int page = 1, int pageSize = 20);
    Task<Producto?> ObtenerAsync(int id);
    Task<Producto> CrearAsync(ProductoCreateDto dto);
    Task<Producto?> ActualizarAsync(int id, ProductoUpdateDto dto);
    Task<bool> EliminarAsync(int id);
}

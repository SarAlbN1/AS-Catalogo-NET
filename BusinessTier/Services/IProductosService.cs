using BusinessTier.DTOs;

namespace BusinessTier.Services;

public interface IProductosService
{
    Task<IEnumerable<ProductoDto>> GetAllAsync();
    Task<ProductoDto?> GetByIdAsync(int id);
    Task<ProductoDto> CreateAsync(ProductoCreateDto dto);
    Task<ProductoDto?> UpdateAsync(int id, ProductoUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}

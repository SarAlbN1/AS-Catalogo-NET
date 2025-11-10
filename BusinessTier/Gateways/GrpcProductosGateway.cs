using BusinessTier.DTOs;
using BusinessTier.GrpcClients;

namespace BusinessTier.Gateways;

public class GrpcProductosGateway : IProductosGateway
{
    private readonly ProductosGrpcClient _cli;
    public GrpcProductosGateway(ProductosGrpcClient cli) => _cli = cli;

    public Task<IEnumerable<ProductoDto>> GetAllAsync() => _cli.GetAllAsync();

    public Task<ProductoDto?> GetByIdAsync(int id) => _cli.GetByIdAsync(id);

    public Task<ProductoDto> CreateAsync(ProductoCreateDto dto) => _cli.CreateAsync(dto);

    public Task<ProductoDto> UpdateAsync(int id, ProductoUpdateDto dto) => _cli.UpdateAsync(id, dto);

    public Task<bool> DeleteAsync(int id) => _cli.DeleteAsync(id);
}

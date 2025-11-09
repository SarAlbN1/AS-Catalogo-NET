using AS_Catalogo_NET.DTOs;
using AS_Catalogo_NET.GrpcClients;

namespace AS_Catalogo_NET.Gateways;

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

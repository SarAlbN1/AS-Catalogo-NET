using System.Net.Http;
using AS_Catalogo_NET.DTOs;
using AS_Catalogo_NET.Protos;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AS_Catalogo_NET.GrpcClients;

public sealed class ProductosGrpcClient
{
    private readonly ProductosData.ProductosDataClient _client;
    private readonly ILogger<ProductosGrpcClient> _logger;
    private readonly TimeSpan _timeout = TimeSpan.FromSeconds(10);

    // Si algún gateway lo necesita, exponemos el client generado
    public ProductosData.ProductosDataClient Client => _client;

    public ProductosGrpcClient(IConfiguration config, ILogger<ProductosGrpcClient> logger)
    {
        _logger = logger;

        // Permite HTTP/2 sin TLS (h2c)
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

        var url = config["GrpcSettings:DataTierUrl"] ?? "http://localhost:5001";
        var handler = new SocketsHttpHandler
        {
            EnableMultipleHttp2Connections = true
        };

        var channel = GrpcChannel.ForAddress(url, new GrpcChannelOptions
        {
            HttpHandler = handler
        });

        _client = new ProductosData.ProductosDataClient(channel);
        _logger.LogInformation("gRPC ProductosDataClient inicializado hacia {Url}", url);
    }

    public async Task<IEnumerable<ProductoDto>> GetAllAsync()
    {
        try
        {
            var resp = await _client.GetAllProductosAsync(new Empty(), deadline: DateTime.UtcNow + _timeout);
            return resp.Productos.Select(Map);
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error gRPC en GetAllProductos: {Status} - {Detail}", ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<ProductoDto?> GetByIdAsync(int id)
    {
        try
        {
            var p = await _client.GetProductoByIdAsync(new ProductoId { Id = id }, deadline: DateTime.UtcNow + _timeout);
            return Map(p);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            _logger.LogWarning("Producto {Id} no encontrado (gRPC NotFound).", id);
            return null;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error gRPC en GetProductoById: {Status} - {Detail}", ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<ProductoDto> CreateAsync(ProductoCreateDto req)
    {
        try
        {
            var r = await _client.CreateProductoAsync(new CreateProductoRequest
            {
                Nombre = req.Nombre,
                Descripcion = req.Descripcion ?? string.Empty,
                Precio = (double)req.Precio,
                CantidadDisponible = req.CantidadDisponible,
                CatalogoId = req.CatalogoId
            }, deadline: DateTime.UtcNow + _timeout);

            return Map(r);
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error gRPC en CreateProducto: {Status} - {Detail}", ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<ProductoDto> UpdateAsync(int id, ProductoUpdateDto req)
    {
        try
        {
            var r = await _client.UpdateProductoAsync(new UpdateProductoRequest
            {
                Id = id,
                Nombre = req.Nombre,
                Descripcion = req.Descripcion ?? string.Empty,
                Precio = (double)req.Precio,
                CantidadDisponible = req.CantidadDisponible
            }, deadline: DateTime.UtcNow + _timeout);

            return Map(r);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            _logger.LogWarning("No se pudo actualizar. Producto {Id} no existe (gRPC NotFound).", id);
            throw;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error gRPC en UpdateProducto: {Status} - {Detail}", ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var r = await _client.DeleteProductoAsync(new ProductoId { Id = id }, deadline: DateTime.UtcNow + _timeout);
            return r.Success;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            _logger.LogWarning("No se pudo eliminar. Producto {Id} no existe (gRPC NotFound).", id);
            return false;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error gRPC en DeleteProducto: {Status} - {Detail}", ex.StatusCode, ex.Status.Detail);
            throw;
        }
    }

    private static ProductoDto Map(Producto p) => new()
    {
        Id = p.Id,
        Nombre = p.Nombre,
        Descripcion = p.Descripcion,
        Precio = (decimal)p.Precio,
        CantidadDisponible = p.CantidadDisponible,
        FechaCreacion = DateTime.TryParse(p.FechaCreacion, out var f) ? f : DateTime.UtcNow,
        CatalogoId = p.CatalogoId
    };
}

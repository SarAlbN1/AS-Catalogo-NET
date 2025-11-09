using Grpc.Core;
using DataTier.Protos;
using DataTier.Data;
using DataTier.KafkaProducer;
using Microsoft.EntityFrameworkCore;
using ProductoEntity = DataTier.Models.Producto;
using ProductoProto = DataTier.Protos.Producto;

namespace DataTier.Services;

public class ProductosGrpcService : ProductosData.ProductosDataBase
{
    private readonly MyAppDbContext _dbContext;
    private readonly ILogger<ProductosGrpcService> _logger;
    private readonly ProductEventProducer _kafkaProducer;

    public ProductosGrpcService(MyAppDbContext dbContext, ILogger<ProductosGrpcService> logger, ProductEventProducer kafkaProducer)
    {
        _dbContext = dbContext;
        _logger = logger;
        _kafkaProducer = kafkaProducer;
    }

    public override async Task<ProductosList> GetAllProductos(Empty request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("Obteniendo todos los productos");

            var productos = await _dbContext.Productos.ToListAsync();

            var response = new ProductosList();
            response.Productos.AddRange(productos.Select(MapToProto));

            _logger.LogInformation($"Se encontraron {productos.Count} productos");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener productos: {ex.Message}");
            throw new RpcException(new Status(StatusCode.Internal, "Error al obtener productos"));
        }
    }

    public override async Task<Producto> GetProductoById(ProductoId request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation($"Buscando producto con ID: {request.Id}");

            var producto = await _dbContext.Productos.FindAsync(request.Id);

            if (producto == null)
            {
                _logger.LogWarning($"Producto con ID {request.Id} no encontrado");
                throw new RpcException(new Status(StatusCode.NotFound, $"Producto con ID {request.Id} no encontrado"));
            }

            return MapToProto(producto);
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener producto: {ex.Message}");
            throw new RpcException(new Status(StatusCode.Internal, "Error al obtener producto"));
        }
    }

    public override async Task<Producto> CreateProducto(CreateProductoRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation($"Creando nuevo producto: {request.Nombre}");

            var producto = new ProductoEntity
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                Precio = (decimal)request.Precio,
                CantidadDisponible = request.CantidadDisponible,
                CatalogoId = request.CatalogoId,
                FechaCreacion = DateTime.UtcNow
            };

            await _dbContext.Productos.AddAsync(producto);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Producto creado con ID: {producto.Id}");

            // Publicar evento a Kafka
            await _kafkaProducer.PublishProductCreatedAsync(producto);

            return MapToProto(producto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al crear producto: {ex.Message}");
            throw new RpcException(new Status(StatusCode.Internal, "Error al crear producto"));
        }
    }

    public override async Task<Producto> UpdateProducto(UpdateProductoRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation($"Actualizando producto con ID: {request.Id}");

            var producto = await _dbContext.Productos.FindAsync(request.Id);

            if (producto == null)
            {
                _logger.LogWarning($"Producto con ID {request.Id} no encontrado");
                throw new RpcException(new Status(StatusCode.NotFound, $"Producto con ID {request.Id} no encontrado"));
            }

            producto.Nombre = request.Nombre;
            producto.Descripcion = request.Descripcion;
            producto.Precio = (decimal)request.Precio;
            producto.CantidadDisponible = request.CantidadDisponible;

            _dbContext.Productos.Update(producto);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Producto con ID {request.Id} actualizado");

            // Publicar evento a Kafka
            await _kafkaProducer.PublishProductUpdatedAsync(producto);

            return MapToProto(producto);
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al actualizar producto: {ex.Message}");
            throw new RpcException(new Status(StatusCode.Internal, "Error al actualizar producto"));
        }
    }

    public override async Task<DeleteResponse> DeleteProducto(ProductoId request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation($"Eliminando producto con ID: {request.Id}");

            var producto = await _dbContext.Productos.FindAsync(request.Id);

            if (producto == null)
            {
                _logger.LogWarning($"Producto con ID {request.Id} no encontrado");
                throw new RpcException(new Status(StatusCode.NotFound, $"Producto con ID {request.Id} no encontrado"));
            }

            var productName = producto.Nombre;
            var productId = producto.Id;

            _dbContext.Productos.Remove(producto);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Producto con ID {request.Id} eliminado");

            // Publicar evento a Kafka
            await _kafkaProducer.PublishProductDeletedAsync(productId, productName);

            return new DeleteResponse
            {
                Success = true,
                Message = $"Producto con ID {request.Id} eliminado exitosamente"
            };
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al eliminar producto: {ex.Message}");
            throw new RpcException(new Status(StatusCode.Internal, "Error al eliminar producto"));
        }
    }

    private ProductoProto MapToProto(ProductoEntity producto)
    {
        return new ProductoProto
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            Precio = (double)producto.Precio,
            CantidadDisponible = producto.CantidadDisponible,
            FechaCreacion = producto.FechaCreacion.ToString("yyyy-MM-ddTHH:mm:ss"),
            CatalogoId = producto.CatalogoId
        };
    }
}

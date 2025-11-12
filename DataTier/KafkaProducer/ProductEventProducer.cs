using Confluent.Kafka;
using System.Text.Json;
using DataTier.Models;

namespace DataTier.KafkaProducer;

public class ProductEventProducer
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<ProductEventProducer> _logger;
    private readonly string _topic;

    public ProductEventProducer(IConfiguration config, ILogger<ProductEventProducer> logger)
    {
        _logger = logger;
        _topic = config["Kafka:Topic"] ?? "product-events";

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = config["Kafka:BootstrapServers"] ?? "localhost:9092",
            ClientId = "datatier-producer",
            Acks = Acks.All,
            EnableIdempotence = true,
            MessageTimeoutMs = 5000
        };

        _producer = new ProducerBuilder<string, string>(producerConfig).Build();
        _logger.LogInformation($"Kafka Producer inicializado - Topic: {_topic}");
    }

    public async Task PublishProductCreatedAsync(Producto producto)
    {
        try
        {
            var productEvent = new
            {
                EventType = "ProductCreated",
                ProductId = producto.Id,
                ProductName = producto.Nombre,
                Descripcion = producto.Descripcion,
                Price = producto.Precio,
                CantidadDisponible = producto.CantidadDisponible,
                CatalogoId = producto.CatalogoId,
                Timestamp = DateTime.UtcNow
            };

            var message = new Message<string, string>
            {
                Key = producto.Id.ToString(),
                Value = JsonSerializer.Serialize(productEvent)
            };

            var result = await _producer.ProduceAsync(_topic, message);

            _logger.LogInformation(
                $"✅ Evento ProductCreated publicado - ID: {producto.Id}, Partition: {result.Partition}, Offset: {result.Offset}");
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError($"❌ Error al publicar evento ProductCreated: {ex.Error.Reason}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error general al publicar evento: {ex.Message}");
            throw;
        }
    }

    public async Task PublishProductUpdatedAsync(Producto producto)
    {
        try
        {
            var productEvent = new
            {
                EventType = "ProductUpdated",
                ProductId = producto.Id,
                ProductName = producto.Nombre,
                Descripcion = producto.Descripcion,
                Price = producto.Precio,
                CantidadDisponible = producto.CantidadDisponible,
                CatalogoId = producto.CatalogoId,
                Timestamp = DateTime.UtcNow
            };

            var message = new Message<string, string>
            {
                Key = producto.Id.ToString(),
                Value = JsonSerializer.Serialize(productEvent)
            };

            var result = await _producer.ProduceAsync(_topic, message);

            _logger.LogInformation(
                $"✅ Evento ProductUpdated publicado - ID: {producto.Id}, Partition: {result.Partition}, Offset: {result.Offset}");
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError($"❌ Error al publicar evento ProductUpdated: {ex.Error.Reason}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error general al publicar evento: {ex.Message}");
            throw;
        }
    }

    public async Task PublishProductDeletedAsync(int productId, string productName)
    {
        try
        {
            var productEvent = new
            {
                EventType = "ProductDeleted",
                ProductId = productId,
                ProductName = productName,
                Timestamp = DateTime.UtcNow
            };

            var message = new Message<string, string>
            {
                Key = productId.ToString(),
                Value = JsonSerializer.Serialize(productEvent)
            };

            var result = await _producer.ProduceAsync(_topic, message);

            _logger.LogInformation(
                $"✅ Evento ProductDeleted publicado - ID: {productId}, Partition: {result.Partition}, Offset: {result.Offset}");
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError($"❌ Error al publicar evento ProductDeleted: {ex.Error.Reason}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error general al publicar evento: {ex.Message}");
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Flush(TimeSpan.FromSeconds(10));
        _producer?.Dispose();
        _logger.LogInformation("Kafka Producer disposed");
    }
}

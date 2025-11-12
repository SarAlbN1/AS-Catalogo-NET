using System.Text.Json;
using Confluent.Kafka;
using KafkaConsumer.Models;
using KafkaConsumer.Services;

namespace KafkaConsumer;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConsumer<string, string> _consumer;
    private readonly EmailService _emailService;
    private readonly IConfiguration _config;

    public Worker(ILogger<Worker> logger, IConfiguration config, EmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
        _config = config;

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = config["Kafka:BootstrapServers"],
            GroupId = config["Kafka:GroupId"],
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        _consumer.Subscribe(config["Kafka:Topic"]);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 Kafka Consumer Worker iniciado...");
        _logger.LogInformation($"📡 Conectado a: {_config["Kafka:BootstrapServers"]}");
        _logger.LogInformation($"📬 Suscrito al topic: {_config["Kafka:Topic"]}");
        _logger.LogInformation($"👥 Group ID: {_config["Kafka:GroupId"]}");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = _consumer.Consume(TimeSpan.FromSeconds(1));

                if (consumeResult != null)
                {
                    _logger.LogInformation($"📨 Mensaje recibido - Key: {consumeResult.Message.Key}");
                    _logger.LogInformation($"📄 Contenido: {consumeResult.Message.Value}");

                    var productEvent = JsonSerializer.Deserialize<ProductEvent>(
                        consumeResult.Message.Value);

                    if (productEvent != null)
                    {
                        await ProcessEvent(productEvent);
                        
                        _consumer.Commit(consumeResult);
                        _logger.LogInformation("✅ Mensaje procesado y commiteado");
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ No se pudo deserializar el mensaje");
                    }
                }
            }
            catch (ConsumeException ex)
            {
                _logger.LogError($"❌ Error consumiendo mensaje: {ex.Error.Reason}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error general: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                await Task.Delay(5000, stoppingToken); // Esperar antes de reintentar
            }
        }
    }

    private async Task ProcessEvent(ProductEvent productEvent)
    {
        _logger.LogInformation($"⚙️ Procesando evento: {productEvent.EventType} para producto: {productEvent.ProductName}");

        switch (productEvent.EventType)
        {
            case "ProductCreated":
                await _emailService.SendProductCreatedEmailAsync(productEvent);
                _logger.LogInformation($"✉️ Email de creación enviado para: {productEvent.ProductName}");
                break;
            
            case "ProductUpdated":
                await _emailService.SendProductUpdatedEmailAsync(productEvent);
                _logger.LogInformation($"✉️ Email de actualización enviado para: {productEvent.ProductName}");
                break;
            
            case "ProductDeleted":
                await _emailService.SendProductDeletedEmailAsync(productEvent);
                _logger.LogInformation($"✉️ Email de eliminación enviado para: {productEvent.ProductName}");
                break;
            
            default:
                _logger.LogWarning($"⚠️ Tipo de evento desconocido: {productEvent.EventType}");
                break;
        }
    }

    public override void Dispose()
    {
        _logger.LogInformation("🛑 Cerrando Kafka Consumer...");
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}

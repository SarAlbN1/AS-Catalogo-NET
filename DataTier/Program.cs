using DataTier.Services;
using DataTier.Data;
using DataTier.KafkaProducer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

// Configure MySQL with Pomelo
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MyAppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Register Kafka Producer as Singleton
builder.Services.AddSingleton<ProductEventProducer>();

// Add logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});

var app = builder.Build();

// Ensure database schema exists on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MyAppDbContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
app.MapGrpcService<ProductosGrpcService>();
app.MapGet("/", () => "DataTier gRPC Server is running on port 5001. Communication with gRPC endpoints must be made through a gRPC client.");

app.Logger.LogInformation("DataTier gRPC Server iniciado en puerto 5001");

app.Run();

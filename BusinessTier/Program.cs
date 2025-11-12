using BusinessTier.Persistance;
using BusinessTier.Services;
using BusinessTier.GrpcClients;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Controllers
builder.Services.AddControllers();

// MySQL (Pomelo)
var connection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost;Port=3306;Database=catalogo_db;User=catalogo_user;Password=catalogo_pass;";
builder.Services.AddDbContext<MyAppDbContext>(opt =>
    opt.UseMySql(connection, ServerVersion.AutoDetect(connection)));

// gRPC Client + Service con fallback
builder.Services.AddScoped<ProductosGrpcClient>();
builder.Services.AddScoped<IProductosService, ProductosService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

// Migraciones automáticas (solo si usa EF)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MyAppDbContext>();
    await db.Database.EnsureCreatedAsync();
}

await app.RunAsync();

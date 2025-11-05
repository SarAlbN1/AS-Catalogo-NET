using AS_Catalogo_NET.Persistance;
using AS_Catalogo_NET.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Controllers
builder.Services.AddControllers();

// MySQL (Pomelo)
var connection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost;Port=3306;Database=catalogo_db;User=root;Password=tu_contraseña;";
builder.Services.AddDbContext<MyAppDbContext>(opt =>
    opt.UseMySql(connection, ServerVersion.AutoDetect(connection)));

// IoC
builder.Services.AddScoped<IProductosService, ProductosService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

// Migraciones automáticas
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MyAppDbContext>();
    await db.Database.MigrateAsync();
}

await app.RunAsync();

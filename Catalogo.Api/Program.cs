using Catalogo.Api.Data;
using Catalogo.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// EF Core (SQLite)
builder.Services.AddDbContext<CatalogoDb>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("CatalogoDb")));

// Servicios de dominio
builder.Services.AddScoped<IProductosService, ProductosService>();

// MVC + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger UI en Dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

// Crear DB si no existe + sembrar datos demo
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogoDb>();
    await db.Database.EnsureCreatedAsync();
    await Seed.CargarAsync(db);
}

await app.RunAsync();

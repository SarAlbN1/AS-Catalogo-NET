using Microsoft.EntityFrameworkCore;
using CatalogoEntity = AS_Catalogo_NET.Model.Catalogo;
using ProductoEntity = AS_Catalogo_NET.Model.Producto;

namespace AS_Catalogo_NET.Persistance;

public class MyAppDbContext : DbContext
{
    public MyAppDbContext(DbContextOptions<MyAppDbContext> options) : base(options) { }

    public DbSet<CatalogoEntity> Catalogos => Set<CatalogoEntity>();
    public DbSet<ProductoEntity> Productos => Set<ProductoEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductoEntity>(e =>
        {
            e.Property(p => p.Nombre).IsRequired().HasMaxLength(120);
            e.Property(p => p.Descripcion).IsRequired().HasMaxLength(500);
            e.Property(p => p.Precio).HasColumnType("decimal(18,2)");
            e.HasOne(p => p.Catalogo)
             .WithMany(c => c.Productos)
             .HasForeignKey(p => p.CatalogoId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Semilla mínima
        modelBuilder.Entity<CatalogoEntity>().HasData(new CatalogoEntity { Id = 1 });
        modelBuilder.Entity<ProductoEntity>().HasData(
            new ProductoEntity { Id = 1, Nombre = "Laptop Dell XPS 15", Descripcion = "i7, 16GB, 512GB", Precio = 1299.99m, CantidadDisponible = 10, CatalogoId = 1, FechaCreacion = DateTime.UtcNow },
            new ProductoEntity { Id = 2, Nombre = "Mouse Logitech MX Master 3", Descripcion = "Inalámbrico", Precio = 99.99m, CantidadDisponible = 50, CatalogoId = 1, FechaCreacion = DateTime.UtcNow }
        );
    }
}

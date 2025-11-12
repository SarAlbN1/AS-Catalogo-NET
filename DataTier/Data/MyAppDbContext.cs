using Microsoft.EntityFrameworkCore;
using DataTier.Models;

namespace DataTier.Data;

public class MyAppDbContext : DbContext
{
    public MyAppDbContext(DbContextOptions<MyAppDbContext> options) : base(options) { }

    public DbSet<Catalogo> Catalogos => Set<Catalogo>();
    public DbSet<Producto> Productos => Set<Producto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>(e =>
        {
            e.Property(p => p.Nombre).IsRequired().HasMaxLength(120);
            e.Property(p => p.Descripcion).IsRequired().HasMaxLength(500);
            e.Property(p => p.Precio).HasColumnType("decimal(18,2)");
            e.HasOne(p => p.Catalogo)
             .WithMany(c => c.Productos)
             .HasForeignKey(p => p.CatalogoId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Semilla mínima - Comentado porque usamos Database First (datos en BD)
        // modelBuilder.Entity<Catalogo>().HasData(new Catalogo { Id = 1 });
        // modelBuilder.Entity<Producto>().HasData(
        //     new Producto { Id = 1, Nombre = "Laptop Dell XPS 15", Descripcion = "i7, 16GB, 512GB", Precio = 1299.99m, CantidadDisponible = 10, CatalogoId = 1, FechaCreacion = DateTime.UtcNow },
        //     new Producto { Id = 2, Nombre = "Mouse Logitech MX Master 3", Descripcion = "Inalámbrico", Precio = 99.99m, CantidadDisponible = 50, CatalogoId = 1, FechaCreacion = DateTime.UtcNow }
        // );
    }
}

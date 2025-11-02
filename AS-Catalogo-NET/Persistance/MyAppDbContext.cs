using System;
using AS_Catalogo_NET.Model;
using Microsoft.EntityFrameworkCore;

namespace AS_Catalogo_NET.Persistance;

public class MyAppDbContext : DbContext
{

    public MyAppDbContext(DbContextOptions<MyAppDbContext> options) : base(options)
    {
    }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Catalogo> Catalogo { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar relación uno a muchos: Un Catálogo tiene muchos Productos
        modelBuilder.Entity<Producto>()
            .HasOne(p => p.Catalogo)
            .WithMany(c => c.Productos)
            .HasForeignKey(p => p.CatalogoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Sembrar un catálogo inicial
        modelBuilder.Entity<Catalogo>().HasData(
            new Catalogo { Id = 1 }
        );

        // Sembrar productos de prueba
        modelBuilder.Entity<Producto>().HasData(
            new Producto
            {
                Id = 1,
                Nombre = "Laptop Dell XPS 15",
                Descripcion = "Laptop de alto rendimiento con procesador Intel i7, 16GB RAM, SSD 512GB",
                Precio = 1299.99m,
                CantidadDisponible = 10,
                FechaCreacion = new DateTime(2024, 1, 15),
                CatalogoId = 1
            },
            new Producto
            {
                Id = 2,
                Nombre = "Mouse Logitech MX Master 3",
                Descripcion = "Mouse ergonómico inalámbrico con sensor de alta precisión",
                Precio = 99.99m,
                CantidadDisponible = 50,
                FechaCreacion = new DateTime(2024, 2, 10),
                CatalogoId = 1
            },
            new Producto
            {
                Id = 3,
                Nombre = "Teclado Mecánico Corsair K95",
                Descripcion = "Teclado mecánico RGB con switches Cherry MX",
                Precio = 189.99m,
                CantidadDisponible = 25,
                FechaCreacion = new DateTime(2024, 2, 20),
                CatalogoId = 1
            },
            new Producto
            {
                Id = 4,
                Nombre = "Monitor Samsung 27\" 4K",
                Descripcion = "Monitor UHD 4K de 27 pulgadas con tecnología HDR",
                Precio = 449.99m,
                CantidadDisponible = 15,
                FechaCreacion = new DateTime(2024, 3, 5),
                CatalogoId = 1
            },
            new Producto
            {
                Id = 5,
                Nombre = "Auriculares Sony WH-1000XM5",
                Descripcion = "Auriculares inalámbricos con cancelación de ruido activa",
                Precio = 349.99m,
                CantidadDisponible = 30,
                FechaCreacion = new DateTime(2024, 3, 15),
                CatalogoId = 1
            },
            new Producto
            {
                Id = 6,
                Nombre = "Webcam Logitech C920",
                Descripcion = "Cámara web Full HD 1080p con micrófono estéreo",
                Precio = 79.99m,
                CantidadDisponible = 40,
                FechaCreacion = new DateTime(2024, 4, 1),
                CatalogoId = 1
            },
            new Producto
            {
                Id = 7,
                Nombre = "SSD Samsung 1TB",
                Descripcion = "Unidad de estado sólido NVMe de 1TB con velocidades de lectura/escritura ultra rápidas",
                Precio = 129.99m,
                CantidadDisponible = 60,
                FechaCreacion = new DateTime(2024, 4, 10),
                CatalogoId = 1
            },
            new Producto
            {
                Id = 8,
                Nombre = "Hub USB-C Anker",
                Descripcion = "Hub USB-C de 7 puertos con puerto HDMI y carga PD",
                Precio = 59.99m,
                CantidadDisponible = 35,
                FechaCreacion = new DateTime(2024, 5, 1),
                CatalogoId = 1
            }
        );
    }
}

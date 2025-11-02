using Catalogo.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.Api.Data;

public static class Seed
{
    public static async Task CargarAsync(CatalogoDb db)
    {
        if (await db.Productos.AnyAsync()) return;

        db.Productos.AddRange(
            new Producto { Nombre = "Teclado Mecánico", Precio = 199_900m, Stock = 12 },
            new Producto { Nombre = "Mouse Inalámbrico", Precio = 89_900m, Stock = 35 },
            new Producto { Nombre = "Monitor 27\"", Precio = 1_299_900m, Stock = 8 }
        );

        await db.SaveChangesAsync();
    }
}

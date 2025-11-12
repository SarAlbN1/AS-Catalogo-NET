using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using ClientApp.DTOs;

namespace ClientApp;

class Program
{
    private static readonly HttpClient httpClient = new HttpClient();
    private static string baseUrl = (Environment.GetEnvironmentVariable("CLIENTAPP_BASEURL") ?? "http://localhost:8080/api").TrimEnd('/');

    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════════════╗");
        Console.WriteLine("║   CLIENTE CATÁLOGO DE PRODUCTOS               ║");
        Console.WriteLine("╚════════════════════════════════════════════════╝");
        Console.ResetColor();
        
        if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
        {
            baseUrl = args[0].TrimEnd('/');
        }

        Console.WriteLine("\nCONFIGURACIÓN:");
        Console.WriteLine($"Usando URL del API: {baseUrl}\n");

        bool salir = false;

        while (!salir)
        {
            MostrarMenu();
            var opcion = Console.ReadLine();

            try
            {
                switch (opcion)
                {
                    case "1":
                        await ListarProductos();
                        break;
                    case "2":
                        await ObtenerProductoPorId();
                        break;
                    case "3":
                        await CrearProducto();
                        break;
                    case "4":
                        await ActualizarProducto();
                        break;
                    case "5":
                        await EliminarProducto();
                        break;
                    case "6":
                        await BuscarProductoPorNombre();
                        break;
                    case "0":
                        salir = true;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nHasta luego!");
                        Console.ResetColor();
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Opción no válida");
                        Console.ResetColor();
                        break;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nError de conexión: {ex.Message}");
                Console.WriteLine("Verifica que el API esté ejecutándose en " + baseUrl);
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nError: {ex.Message}");
                Console.ResetColor();
            }

            if (!salir)
            {
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }

    static void MostrarMenu()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n╔════════════════════════════════════════════════╗");
        Console.WriteLine("║              MENÚ PRINCIPAL                    ║");
        Console.WriteLine("╚════════════════════════════════════════════════╝");
        Console.ResetColor();
        
        Console.WriteLine("\nCONSULTAS:");
        Console.WriteLine("  (1) - Listar todos los productos");
        Console.WriteLine("  (2) - Buscar producto por ID");
        Console.WriteLine("  (6) - Buscar producto por nombre");
        
        Console.WriteLine("\nOPERACIONES:");
        Console.WriteLine("  (3) - Crear nuevo producto");
        Console.WriteLine("  (4) - Actualizar producto");
        Console.WriteLine("  (5) - Eliminar producto");
        
        Console.WriteLine("\n  (0) - Salir");
        
        Console.Write("\nSeleccione una opción: ");
    }

    static async Task ListarProductos()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\nLISTANDO TODOS LOS PRODUCTOS...\n");
        Console.ResetColor();

        var response = await httpClient.GetAsync($"{baseUrl}/productos");
        
        if (response.IsSuccessStatusCode)
        {
            var productos = await response.Content.ReadFromJsonAsync<List<ProductoDto>>();
            
            if (productos == null || productos.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No hay productos registrados");
                Console.ResetColor();
                return;
            }

            Console.WriteLine("┌────┬─────────────────────────┬──────────────┬─────────┐");
            Console.WriteLine("│ ID │ Nombre                  │ Precio       │ Stock   │");
            Console.WriteLine("├────┼─────────────────────────┼──────────────┼─────────┤");
            
            foreach (var producto in productos)
            {
                Console.WriteLine($"│ {producto.Id,-2} │ {Truncate(producto.Nombre, 23),-23} │ ${producto.Precio,-11:F2} │ {producto.CantidadDisponible,-7} │");
            }
            
            Console.WriteLine("└────┴─────────────────────────┴──────────────┴─────────┘");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nTotal de productos: {productos.Count}");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error al obtener productos: {response.StatusCode}");
            Console.ResetColor();
        }
    }

    static async Task ObtenerProductoPorId()
    {
        Console.Write("\nIngrese el ID del producto: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ID inválido");
            Console.ResetColor();
            return;
        }

        var response = await httpClient.GetAsync($"{baseUrl}/productos/{id}");
        
        if (response.IsSuccessStatusCode)
        {
            var producto = await response.Content.ReadFromJsonAsync<ProductoDto>();
            
            if (producto != null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nPRODUCTO ENCONTRADO:\n");
                Console.ResetColor();
                MostrarDetalleProducto(producto);
            }
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Producto con ID {id} no encontrado");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {response.StatusCode}");
            Console.ResetColor();
        }
    }

    static async Task CrearProducto()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\nCREAR NUEVO PRODUCTO\n");
        Console.ResetColor();

        var nuevoProducto = new ProductoCreateDto();

        Console.Write("Nombre: ");
        nuevoProducto.Nombre = Console.ReadLine() ?? "";

        Console.Write("Descripción: ");
        nuevoProducto.Descripcion = Console.ReadLine() ?? "";

        Console.Write("Precio: $");
        var precioInput = Console.ReadLine();
        if (!decimal.TryParse(precioInput, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal precio))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Precio inválido");
            Console.ResetColor();
            return;
        }
        nuevoProducto.Precio = precio;

        Console.Write("Stock: ");
        var stockInput = Console.ReadLine();
        if (!int.TryParse(stockInput, NumberStyles.Integer, CultureInfo.CurrentCulture, out int stock))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Stock inválido");
            Console.ResetColor();
            return;
        }
        nuevoProducto.CantidadDisponible = stock;

        Console.Write("Catálogo ID (Enter para 1): ");
        var catalogoInput = Console.ReadLine();
        if (!int.TryParse(catalogoInput, NumberStyles.Integer, CultureInfo.CurrentCulture, out int catalogoId) || catalogoId <= 0)
        {
            catalogoId = 1;
        }
        nuevoProducto.CatalogoId = catalogoId;

        var response = await httpClient.PostAsJsonAsync($"{baseUrl}/productos", nuevoProducto);
        
        if (response.IsSuccessStatusCode)
        {
            var productoCreado = await response.Content.ReadFromJsonAsync<ProductoDto>();
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nPRODUCTO CREADO EXITOSAMENTE");
            Console.ResetColor();
            
            if (productoCreado != null)
            {
                MostrarDetalleProducto(productoCreado);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\nSe enviará un email de notificación...");
                Console.ResetColor();
            }
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error al crear producto: {response.StatusCode}");
            Console.WriteLine($"Detalle: {error}");
            Console.ResetColor();
        }
    }

    static async Task ActualizarProducto()
    {
        Console.Write("\nIngrese el ID del producto a actualizar: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ID inválido");
            Console.ResetColor();
            return;
        }

        // Primero obtener el producto actual
        var getResponse = await httpClient.GetAsync($"{baseUrl}/productos/{id}");
        if (!getResponse.IsSuccessStatusCode)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Producto con ID {id} no encontrado");
            Console.ResetColor();
            return;
        }

        var productoActual = await getResponse.Content.ReadFromJsonAsync<ProductoDto>();
        
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\nACTUALIZAR PRODUCTO\n");
        Console.WriteLine("(Presione Enter para mantener el valor actual)\n");
        Console.ResetColor();

        var productoActualizado = new ProductoUpdateDto();

        Console.Write($"Nombre [{productoActual?.Nombre}]: ");
        var nombre = Console.ReadLine();
        productoActualizado.Nombre = string.IsNullOrWhiteSpace(nombre) ? productoActual?.Nombre ?? "" : nombre;

        Console.Write($"Descripción [{productoActual?.Descripcion}]: ");
        var descripcion = Console.ReadLine();
        productoActualizado.Descripcion = string.IsNullOrWhiteSpace(descripcion) ? productoActual?.Descripcion ?? "" : descripcion;

        Console.Write($"Precio [${productoActual?.Precio:F2}]: $");
        var precioStr = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(precioStr))
        {
            productoActualizado.Precio = productoActual?.Precio ?? 0;
        }
        else if (!decimal.TryParse(precioStr, NumberStyles.Number, CultureInfo.CurrentCulture, out var precioNuevo))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Precio inválido");
            Console.ResetColor();
            return;
        }
        else
        {
            productoActualizado.Precio = precioNuevo;
        }

        Console.Write($"Stock [{productoActual?.CantidadDisponible}]: ");
        var stockStr = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(stockStr))
        {
            productoActualizado.CantidadDisponible = productoActual?.CantidadDisponible ?? 0;
        }
        else if (!int.TryParse(stockStr, NumberStyles.Integer, CultureInfo.CurrentCulture, out var stockNuevo))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Stock inválido");
            Console.ResetColor();
            return;
        }
        else
        {
            productoActualizado.CantidadDisponible = stockNuevo;
        }

        var response = await httpClient.PutAsJsonAsync($"{baseUrl}/productos/{id}", productoActualizado);
        
        if (response.IsSuccessStatusCode)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nPRODUCTO ACTUALIZADO EXITOSAMENTE");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Se enviará un email de notificación...");
            Console.ResetColor();
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error al actualizar producto: {response.StatusCode}");
            Console.WriteLine($"Detalle: {error}");
            Console.ResetColor();
        }
    }

    static async Task EliminarProducto()
    {
        Console.Write("\nIngrese el ID del producto a eliminar: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ID inválido");
            Console.ResetColor();
            return;
        }

        // Primero mostrar el producto
        var getResponse = await httpClient.GetAsync($"{baseUrl}/productos/{id}");
        if (!getResponse.IsSuccessStatusCode)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Producto con ID {id} no encontrado");
            Console.ResetColor();
            return;
        }

        var producto = await getResponse.Content.ReadFromJsonAsync<ProductoDto>();
        
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\nADVERTENCIA: Está a punto de eliminar el siguiente producto:");
        Console.ResetColor();
        MostrarDetalleProducto(producto!);

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("\n¿Está seguro? (S/N): ");
        Console.ResetColor();
        var confirmacion = Console.ReadLine()?.ToUpper();

        if (confirmacion != "S")
        {
            Console.WriteLine("Operación cancelada");
            return;
        }

        var response = await httpClient.DeleteAsync($"{baseUrl}/productos/{id}");
        
        if (response.IsSuccessStatusCode)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nPRODUCTO ELIMINADO EXITOSAMENTE");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Se enviará un email de notificación...");
            Console.ResetColor();
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error al eliminar producto: {response.StatusCode}");
            Console.WriteLine($"Detalle: {error}");
            Console.ResetColor();
        }
    }

    static async Task BuscarProductoPorNombre()
    {
        Console.Write("\nIngrese el nombre (o parte del nombre): ");
        var nombre = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(nombre))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Debe ingresar un nombre");
            Console.ResetColor();
            return;
        }

        var response = await httpClient.GetAsync($"{baseUrl}/productos");
        
        if (response.IsSuccessStatusCode)
        {
            var productos = await response.Content.ReadFromJsonAsync<List<ProductoDto>>();
            var resultados = productos?.Where(p => p.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase)).ToList();

            if (resultados == null || resultados.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"No se encontraron productos con el nombre '{nombre}'");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nSe encontraron {resultados.Count} producto(s):\n");
            Console.ResetColor();

            Console.WriteLine("┌────┬─────────────────────────┬──────────────┬─────────┐");
            Console.WriteLine("│ ID │ Nombre                  │ Precio       │ Stock   │");
            Console.WriteLine("├────┼─────────────────────────┼──────────────┼─────────┤");
            
            foreach (var producto in resultados)
            {
                Console.WriteLine($"│ {producto.Id,-2} │ {Truncate(producto.Nombre, 23),-23} │ ${producto.Precio,-11:F2} │ {producto.CantidadDisponible,-7} │");
            }
            
            Console.WriteLine("└────┴─────────────────────────┴──────────────┴─────────┘");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error al buscar productos: {response.StatusCode}");
            Console.ResetColor();
        }
    }

    static void MostrarDetalleProducto(ProductoDto producto)
    {
        var fechaLocal = producto.FechaCreacion.ToLocalTime().ToString("g", CultureInfo.CurrentCulture);

        Console.WriteLine("┌─────────────────────────────────────────────┐");
        Console.WriteLine($"│ ID:          {producto.Id,-30} │");
        Console.WriteLine($"│ Nombre:      {Truncate(producto.Nombre, 30),-30} │");
        Console.WriteLine($"│ Descripción: {Truncate(producto.Descripcion, 30),-30} │");
        Console.WriteLine($"│ Precio:      ${producto.Precio,-29:F2} │");
        Console.WriteLine($"│ Stock:       {producto.CantidadDisponible,-30} │");
        Console.WriteLine($"│ Catálogo ID: {producto.CatalogoId,-30} │");
        Console.WriteLine($"│ Creado:      {Truncate(fechaLocal, 30),-30} │");
        Console.WriteLine("└─────────────────────────────────────────────┘");
    }

    static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength - 3) + "...";
    }
}

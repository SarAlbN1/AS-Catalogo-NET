using System.Net.Http.Json;
using System.Text.Json;
using ClientApp.DTOs;

namespace ClientApp;

class Program
{
    private static readonly HttpClient httpClient = new HttpClient();
    private static string baseUrl = "http://localhost:8080/api";

    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════════════╗");
        Console.WriteLine("║   📦 CLIENTE CATÁLOGO DE PRODUCTOS 📦         ║");
        Console.WriteLine("╚════════════════════════════════════════════════╝");
        Console.ResetColor();
        
        Console.WriteLine("\n🔧 Configuración:");
        Console.Write("Ingrese la URL del API (Enter para usar http://localhost:8080/api): ");
        var url = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(url))
        {
            baseUrl = url.TrimEnd('/');
        }
        Console.WriteLine($"✅ Usando URL: {baseUrl}\n");

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
                        Console.WriteLine("\n👋 ¡Hasta luego!");
                        Console.ResetColor();
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("❌ Opción no válida");
                        Console.ResetColor();
                        break;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n❌ Error de conexión: {ex.Message}");
                Console.WriteLine("Verifica que el API esté ejecutándose en " + baseUrl);
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n❌ Error: {ex.Message}");
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
        
        Console.WriteLine("\n📋 CONSULTAS:");
        Console.WriteLine("  1️⃣  - Listar todos los productos");
        Console.WriteLine("  2️⃣  - Buscar producto por ID");
        Console.WriteLine("  6️⃣  - Buscar producto por nombre");
        
        Console.WriteLine("\n✏️  OPERACIONES:");
        Console.WriteLine("  3️⃣  - Crear nuevo producto");
        Console.WriteLine("  4️⃣  - Actualizar producto");
        Console.WriteLine("  5️⃣  - Eliminar producto");
        
        Console.WriteLine("\n  0️⃣  - Salir");
        
        Console.Write("\n👉 Seleccione una opción: ");
    }

    static async Task ListarProductos()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n📋 LISTANDO TODOS LOS PRODUCTOS...\n");
        Console.ResetColor();

        var response = await httpClient.GetAsync($"{baseUrl}/productos");
        
        if (response.IsSuccessStatusCode)
        {
            var productos = await response.Content.ReadFromJsonAsync<List<ProductoDto>>();
            
            if (productos == null || productos.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("⚠️  No hay productos registrados");
                Console.ResetColor();
                return;
            }

            Console.WriteLine("┌────┬─────────────────────────┬──────────────┬─────────┐");
            Console.WriteLine("│ ID │ Nombre                  │ Precio       │ Stock   │");
            Console.WriteLine("├────┼─────────────────────────┼──────────────┼─────────┤");
            
            foreach (var producto in productos)
            {
                Console.WriteLine($"│ {producto.Id,-2} │ {Truncate(producto.Nombre, 23),-23} │ ${producto.Precio,-11:F2} │ {producto.Stock,-7} │");
            }
            
            Console.WriteLine("└────┴─────────────────────────┴──────────────┴─────────┘");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n✅ Total de productos: {productos.Count}");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Error al obtener productos: {response.StatusCode}");
            Console.ResetColor();
        }
    }

    static async Task ObtenerProductoPorId()
    {
        Console.Write("\n🔍 Ingrese el ID del producto: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ ID inválido");
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
                Console.WriteLine("\n✅ PRODUCTO ENCONTRADO:\n");
                Console.ResetColor();
                MostrarDetalleProducto(producto);
            }
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠️  Producto con ID {id} no encontrado");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Error: {response.StatusCode}");
            Console.ResetColor();
        }
    }

    static async Task CrearProducto()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n➕ CREAR NUEVO PRODUCTO\n");
        Console.ResetColor();

        var nuevoProducto = new ProductoCreateDto();

        Console.Write("📝 Nombre: ");
        nuevoProducto.Nombre = Console.ReadLine() ?? "";

        Console.Write("📄 Descripción: ");
        nuevoProducto.Descripcion = Console.ReadLine() ?? "";

        Console.Write("💰 Precio: $");
        if (!decimal.TryParse(Console.ReadLine(), out decimal precio))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ Precio inválido");
            Console.ResetColor();
            return;
        }
        nuevoProducto.Precio = precio;

        Console.Write("📦 Stock: ");
        if (!int.TryParse(Console.ReadLine(), out int stock))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ Stock inválido");
            Console.ResetColor();
            return;
        }
        nuevoProducto.Stock = stock;

        var response = await httpClient.PostAsJsonAsync($"{baseUrl}/productos", nuevoProducto);
        
        if (response.IsSuccessStatusCode)
        {
            var productoCreado = await response.Content.ReadFromJsonAsync<ProductoDto>();
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n🎉 ¡PRODUCTO CREADO EXITOSAMENTE!");
            Console.ResetColor();
            
            if (productoCreado != null)
            {
                MostrarDetalleProducto(productoCreado);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n📧 Se enviará un email de notificación...");
                Console.ResetColor();
            }
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Error al crear producto: {response.StatusCode}");
            Console.WriteLine($"Detalle: {error}");
            Console.ResetColor();
        }
    }

    static async Task ActualizarProducto()
    {
        Console.Write("\n🔍 Ingrese el ID del producto a actualizar: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ ID inválido");
            Console.ResetColor();
            return;
        }

        // Primero obtener el producto actual
        var getResponse = await httpClient.GetAsync($"{baseUrl}/productos/{id}");
        if (!getResponse.IsSuccessStatusCode)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠️  Producto con ID {id} no encontrado");
            Console.ResetColor();
            return;
        }

        var productoActual = await getResponse.Content.ReadFromJsonAsync<ProductoDto>();
        
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n✏️  ACTUALIZAR PRODUCTO\n");
        Console.WriteLine("(Presione Enter para mantener el valor actual)\n");
        Console.ResetColor();

        var productoActualizado = new ProductoUpdateDto();

        Console.Write($"📝 Nombre [{productoActual?.Nombre}]: ");
        var nombre = Console.ReadLine();
        productoActualizado.Nombre = string.IsNullOrWhiteSpace(nombre) ? productoActual?.Nombre ?? "" : nombre;

        Console.Write($"📄 Descripción [{productoActual?.Descripcion}]: ");
        var descripcion = Console.ReadLine();
        productoActualizado.Descripcion = string.IsNullOrWhiteSpace(descripcion) ? productoActual?.Descripcion ?? "" : descripcion;

        Console.Write($"💰 Precio [${productoActual?.Precio:F2}]: $");
        var precioStr = Console.ReadLine();
        productoActualizado.Precio = string.IsNullOrWhiteSpace(precioStr) ? productoActual?.Precio ?? 0 : decimal.Parse(precioStr);

        Console.Write($"📦 Stock [{productoActual?.Stock}]: ");
        var stockStr = Console.ReadLine();
        productoActualizado.Stock = string.IsNullOrWhiteSpace(stockStr) ? productoActual?.Stock ?? 0 : int.Parse(stockStr);

        var response = await httpClient.PutAsJsonAsync($"{baseUrl}/productos/{id}", productoActualizado);
        
        if (response.IsSuccessStatusCode)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n✅ ¡PRODUCTO ACTUALIZADO EXITOSAMENTE!");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("📧 Se enviará un email de notificación...");
            Console.ResetColor();
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Error al actualizar producto: {response.StatusCode}");
            Console.WriteLine($"Detalle: {error}");
            Console.ResetColor();
        }
    }

    static async Task EliminarProducto()
    {
        Console.Write("\n🗑️  Ingrese el ID del producto a eliminar: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ ID inválido");
            Console.ResetColor();
            return;
        }

        // Primero mostrar el producto
        var getResponse = await httpClient.GetAsync($"{baseUrl}/productos/{id}");
        if (!getResponse.IsSuccessStatusCode)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠️  Producto con ID {id} no encontrado");
            Console.ResetColor();
            return;
        }

        var producto = await getResponse.Content.ReadFromJsonAsync<ProductoDto>();
        
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n⚠️  Está a punto de eliminar el siguiente producto:");
        Console.ResetColor();
        MostrarDetalleProducto(producto!);

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("\n¿Está seguro? (S/N): ");
        Console.ResetColor();
        var confirmacion = Console.ReadLine()?.ToUpper();

        if (confirmacion != "S")
        {
            Console.WriteLine("❌ Operación cancelada");
            return;
        }

        var response = await httpClient.DeleteAsync($"{baseUrl}/productos/{id}");
        
        if (response.IsSuccessStatusCode)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n✅ ¡PRODUCTO ELIMINADO EXITOSAMENTE!");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("📧 Se enviará un email de notificación...");
            Console.ResetColor();
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Error al eliminar producto: {response.StatusCode}");
            Console.WriteLine($"Detalle: {error}");
            Console.ResetColor();
        }
    }

    static async Task BuscarProductoPorNombre()
    {
        Console.Write("\n🔍 Ingrese el nombre (o parte del nombre): ");
        var nombre = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(nombre))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ Debe ingresar un nombre");
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
                Console.WriteLine($"⚠️  No se encontraron productos con el nombre '{nombre}'");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n✅ Se encontraron {resultados.Count} producto(s):\n");
            Console.ResetColor();

            Console.WriteLine("┌────┬─────────────────────────┬──────────────┬─────────┐");
            Console.WriteLine("│ ID │ Nombre                  │ Precio       │ Stock   │");
            Console.WriteLine("├────┼─────────────────────────┼──────────────┼─────────┤");
            
            foreach (var producto in resultados)
            {
                Console.WriteLine($"│ {producto.Id,-2} │ {Truncate(producto.Nombre, 23),-23} │ ${producto.Precio,-11:F2} │ {producto.Stock,-7} │");
            }
            
            Console.WriteLine("└────┴─────────────────────────┴──────────────┴─────────┘");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Error al buscar productos: {response.StatusCode}");
            Console.ResetColor();
        }
    }

    static void MostrarDetalleProducto(ProductoDto producto)
    {
        Console.WriteLine("┌─────────────────────────────────────────────┐");
        Console.WriteLine($"│ ID:          {producto.Id,-30} │");
        Console.WriteLine($"│ Nombre:      {Truncate(producto.Nombre, 30),-30} │");
        Console.WriteLine($"│ Descripción: {Truncate(producto.Descripcion, 30),-30} │");
        Console.WriteLine($"│ Precio:      ${producto.Precio,-29:F2} │");
        Console.WriteLine($"│ Stock:       {producto.Stock,-30} │");
        Console.WriteLine("└─────────────────────────────────────────────┘");
    }

    static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength - 3) + "...";
    }
}

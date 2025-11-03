namespace Catalogo.Api.DTOs;

public class ProductoCreateDto
{
    public string Nombre { get; set; } = default!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

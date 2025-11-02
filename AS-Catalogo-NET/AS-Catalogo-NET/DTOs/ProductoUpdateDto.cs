namespace Catalogo.Api.DTOs;

public class ProductoUpdateDto
{
    public string Nombre { get; set; } = default!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public bool Activo { get; set; }
}

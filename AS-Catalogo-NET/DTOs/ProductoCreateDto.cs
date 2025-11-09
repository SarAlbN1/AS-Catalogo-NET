namespace AS_Catalogo_NET.DTOs;

public class ProductoCreateDto
{
    public string Nombre { get; set; } = default!;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int CantidadDisponible { get; set; }
    public int CatalogoId { get; set; }
}

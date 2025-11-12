namespace BusinessTier.DTOs;

public class ProductoUpdateDto
{
    public string Nombre { get; set; } = default!;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int CantidadDisponible { get; set; }
}

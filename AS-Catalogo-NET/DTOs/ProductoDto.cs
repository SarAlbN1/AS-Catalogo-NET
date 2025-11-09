namespace AS_Catalogo_NET.DTOs;

public class ProductoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = default!;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int CantidadDisponible { get; set; }
    public DateTime FechaCreacion { get; set; }
    public int CatalogoId { get; set; }
}

namespace DataTier.Models;

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public decimal Precio { get; set; }
    public int CantidadDisponible { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public int CatalogoId { get; set; }
    public Catalogo? Catalogo { get; set; }
}

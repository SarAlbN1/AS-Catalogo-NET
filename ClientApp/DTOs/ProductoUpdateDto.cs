namespace ClientApp.DTOs;

public class ProductoUpdateDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int CantidadDisponible { get; set; }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace AS_Catalogo_NET.Model;

public class Producto
{
    [Key]
    public int Id { get; set; }
    [Required]
    public required string Nombre { get; set; }
    public required string Descripcion { get; set; }
    [Required]
    public decimal Precio { get; set; }
    [Required]
    public int CantidadDisponible { get; set; }

    public DateTime FechaCreacion { get; set; }
    
    // Relación con Catálogo
    public int CatalogoId { get; set; }
    public virtual Catalogo? Catalogo { get; set; }
}

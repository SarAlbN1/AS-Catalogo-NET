using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataTier.Models;

[Table("Producto")]
[Index("Activo", Name = "idx_producto_activo")]
[Index("CatalogoId", Name = "idx_producto_catalogo")]
[Index("Nombre", Name = "idx_producto_nombre")]
[Index("Precio", Name = "idx_producto_precio")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class Producto
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string Nombre { get; set; } = null!;

    [StringLength(500)]
    public string Descripcion { get; set; } = null!;

    [Precision(18, 2)]
    public decimal Precio { get; set; }

    public int CantidadDisponible { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaActualizacion { get; set; }

    public int CatalogoId { get; set; }

    [Required]
    public bool? Activo { get; set; }

    [ForeignKey("CatalogoId")]
    [InverseProperty("Productos")]
    public virtual Catalogo Catalogo { get; set; } = null!;
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataTier.Models;

[Table("Catalogo")]
[Index("Activo", Name = "idx_catalogo_activo")]
[Index("Nombre", Name = "idx_catalogo_nombre")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class Catalogo
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string Nombre { get; set; } = null!;

    [StringLength(500)]
    public string? Descripcion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    [Required]
    public bool? Activo { get; set; }

    [InverseProperty("Catalogo")]
    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}

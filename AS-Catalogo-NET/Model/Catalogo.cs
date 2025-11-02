using System;

namespace AS_Catalogo_NET.Model;

public class Catalogo
{
    public int Id { get; set; }
    
    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();

}

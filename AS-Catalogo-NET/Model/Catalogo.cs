namespace AS_Catalogo_NET.Model;

public class Catalogo
{
    public int Id { get; set; }
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}

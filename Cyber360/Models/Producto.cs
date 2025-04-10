using System;
using System.Collections.Generic;

namespace Cyber360.Models;

public partial class Producto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal Precio { get; set; }

    public int Cantidad { get; set; }

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();

    // Clave foránea (debe ser int)
    public int? CategoriaId { get; set; }
    
    // Propiedad de navegación
    public virtual CatProduct Categoria { get; set; }
}

using System;
using System.Collections.Generic;

namespace Cyber360.Models;

public partial class Servicio
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal Precio { get; set; }

    public string? Detalles { get; set; }

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();

    public int? CategoriaId { get; set; }

    // Propiedad de navegación
    public virtual CatServi Categoria { get; set; }
}

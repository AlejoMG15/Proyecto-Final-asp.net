using System;
using System.Collections.Generic;

namespace Cyber360.Models;

public partial class CatProduct
{
    public int Id { get; set; }
    public string Nombre { get; set; }

    // Propiedad Registros debe tener getter y setter
    public int? Registros { get; set; }

    // Propiedad de navegación (opcional)
    public virtual ICollection<Producto> Productos { get; set; }
}


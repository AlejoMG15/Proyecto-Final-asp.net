using System;
using System.Collections.Generic;

namespace Cyber360.Models;

public partial class CatServi
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;
    public int? Registros { get; set; }

    // Propiedad de navegación (opcional)
    public virtual ICollection<Servicio> Servicios { get; set; }
}

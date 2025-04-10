using System;
using System.Collections.Generic;

namespace Cyber360.Models;

public partial class Cliente
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public int? Telefono { get; set; }

    public int? Documento { get; set; }

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}

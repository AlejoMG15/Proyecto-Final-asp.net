using System;
using System.Collections.Generic;

namespace Cyber360.Models;

public partial class Compra
{
    public int Id { get; set; }

    public int ProveedorId { get; set; }

    public int Cantidad { get; set; }

    public decimal Total { get; set; }

    public DateTime? Fecha { get; set; }

    public virtual Proveedore Proveedor { get; set; } = null!;
}

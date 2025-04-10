using System;
using System.Collections.Generic;

namespace Cyber360.Models;

public partial class Proveedore
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Correo { get; set; }

    public string? NombredeEmpresa { get; set; }

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();
}

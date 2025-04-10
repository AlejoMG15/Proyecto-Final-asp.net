using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cyber360.Models;

public partial class Venta
{
    public int Id { get; set; }

    public int? ClienteId { get; set; }

    public int Cantidad { get; set; }

    public DateTime? Fecha { get; set; }

    public decimal? valor { get; set; }

    public virtual Cliente? Cliente { get; set; }

    public int? ServicioId { get; set; }  // Cambia a nullable
    [ForeignKey("ServicioId")]
    public virtual Servicio? Servicio { get; set; }  // Hacer nullable

    public int? ProductoId { get; set; }  // También considera hacerlo nullable si aplica
    [ForeignKey("ProductoId")]
    public virtual Producto? Producto { get; set; }

    public int? CantidadServicio { get; set; }


    public string Estado { get; set; } = "Activa"; // "Activa", "Anulada"
    public DateTime? FechaAnulacion { get; set; }
}

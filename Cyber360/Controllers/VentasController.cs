using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cyber360.Models;

namespace Cyber360.Controllers
{
    public class VentasController : Controller
    {
        private readonly ProyectoCyberContext _context;
        private readonly ILogger<VentasController> _logger;

        public VentasController(ProyectoCyberContext context, ILogger<VentasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Ventas
        public async Task<IActionResult> Index()
        {
            var proyectoCyberContext = _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Producto)
                .Include(v => v.Servicio);
            return View(await proyectoCyberContext.ToListAsync());
        }

        // GET: Ventas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Producto)
                .Include(v => v.Servicio)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        // GET: Ventas/Create
        public IActionResult Create()
        {
            try
            {
                ViewData["ClienteId"] = GetClientesSelectList();
                ViewData["ProductoId"] = GetProductosSelectList();
                ViewData["ServicioId"] = GetServiciosSelectList();

                return View(new Venta
                {
                    Fecha = DateTime.Now,
                    Estado = "Activa",
                    Cantidad = 0,
                    CantidadServicio = 1
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar datos para nueva venta");
                TempData["ErrorMessage"] = "Error al cargar los datos necesarios para crear una venta.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Ventas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClienteId,ProductoId,ServicioId,Cantidad,CantidadServicio")] Venta venta)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Validación básica - al menos producto o servicio
                    if (!venta.ProductoId.HasValue && !venta.ServicioId.HasValue)
                    {
                        ModelState.AddModelError("", "Debes seleccionar al menos un producto o un servicio.");
                        ReloadDropdowns();
                        return View(venta);
                    }

                    // Procesar servicio si existe - VERSIÓN SIMPLIFICADA
                    if (venta.ServicioId.HasValue)
                    {
                        var servicio = await _context.Servicios.FindAsync(venta.ServicioId);
                        if (servicio == null)
                        {
                            ModelState.AddModelError("ServicioId", "El servicio seleccionado no existe.");
                            ReloadDropdowns();
                            return View(venta);
                        }

                        // Validación básica de cantidad (igual que productos)
                        var cantidadServicio = venta.CantidadServicio ?? 1;
                        if (cantidadServicio <= 0)
                        {
                            ModelState.AddModelError("CantidadServicio", "La cantidad debe ser mayor que cero.");
                            ReloadDropdowns();
                            return View(venta);
                        }

                        // Cálculo simple del valor (sin validar disponibilidad)
                        venta.valor = cantidadServicio * servicio.Precio;
                    }
                    else
                    {
                        venta.CantidadServicio = null;
                    }

                    // Procesar producto si existe (mantenido igual)
                    if (venta.ProductoId.HasValue)
                    {
                        var producto = await _context.Productos.FindAsync(venta.ProductoId);
                        if (producto == null)
                        {
                            ModelState.AddModelError("ProductoId", "El producto seleccionado no existe.");
                            ReloadDropdowns();
                            return View(venta);
                        }

                        if (venta.Cantidad <= 0)
                        {
                            ModelState.AddModelError("Cantidad", "La cantidad debe ser mayor que cero.");
                            ReloadDropdowns();
                            return View(venta);
                        }

                        if (venta.Cantidad > producto.Cantidad)
                        {
                            ModelState.AddModelError("Cantidad", $"No hay suficiente stock. Disponible: {producto.Cantidad}");
                            ReloadDropdowns();
                            return View(venta);
                        }

                        producto.Cantidad -= venta.Cantidad;
                        venta.valor = (venta.valor ?? 0) + (venta.Cantidad * producto.Precio);
                    }
                    else
                    {
                        venta.Cantidad = 0;
                    }

                    // Resto del código igual...
                    venta.Fecha = DateTime.Now;
                    venta.Estado = "Activa";

                    _context.Add(venta);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Venta registrada correctamente!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar venta");
                ModelState.AddModelError("", "Ocurrió un error al procesar la venta.");
            }

            ReloadDropdowns();
            return View(venta);
        }

        // Métodos auxiliares para cargar dropdowns
        private SelectList GetClientesSelectList(object selectedValue = null)
        {
            var clientes = _context.Clientes
                .Where(c => c.Id != null)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Nombre} {c.Apellido}"
                })
                .ToList();

            return new SelectList(clientes, "Value", "Text", selectedValue);
        }

        private SelectList GetProductosSelectList(object selectedValue = null)
        {
            var productos = _context.Productos
                .Where(p => p.Id != null && p.Cantidad > 0)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Nombre} (Stock: {p.Cantidad})"
                })
                .ToList();

            return new SelectList(productos, "Value", "Text", selectedValue);
        }

        private SelectList GetServiciosSelectList(object selectedValue = null)
        {
            var servicios = _context.Servicios
                .Where(s => s.Id != null) // Solo filtramos que no sea nulo, igual que productos
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.Nombre} - ${s.Precio:n2}" // Mostramos nombre y precio
                })
                .ToList();

            return new SelectList(servicios, "Value", "Text", selectedValue);
        }

        private void ReloadDropdowns(Venta venta = null)
        {
            ViewData["ClienteId"] = GetClientesSelectList(venta?.ClienteId);
            ViewData["ProductoId"] = GetProductosSelectList(venta?.ProductoId);
            ViewData["ServicioId"] = GetServiciosSelectList(venta?.ServicioId);
        }

        // GET: Ventas/ToggleStatus/5
        public async Task<IActionResult> ToggleStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Producto)
                .Include(v => v.Servicio)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        // POST: Ventas/ToggleStatus/5
        [HttpPost, ActionName("ToggleStatus")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatusConfirmed(int id, string motivo)
        {
            var venta = await _context.Ventas
                .Include(v => v.Producto)
                .Include(v => v.Servicio)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta == null)
            {
                return NotFound();
            }

            if (venta.Estado == "Activa")
            {
                // Anular la venta
                venta.Estado = "Anulada";
                venta.FechaAnulacion = DateTime.Now;
                // Devolver productos al stock si existe
                if (venta.Producto != null && venta.Cantidad > 0)
                {
                    venta.Producto.Cantidad += venta.Cantidad;
                    _context.Update(venta.Producto);
                }

                // Para servicios, podrías implementar lógica similar si es necesario
                // (por ejemplo, liberar cupos o disponibilidad)

                TempData["SuccessMessage"] = $"Venta anulada correctamente. Motivo: {motivo}";
            }
            else
            {
                // Reactivar la venta
                venta.Estado = "Activa";
                venta.FechaAnulacion = null;

                // Restar productos del stock si existe
                if (venta.Producto != null && venta.Cantidad > 0)
                {
                    if (venta.Producto.Cantidad < venta.Cantidad)
                    {
                        TempData["ErrorMessage"] = "No hay suficiente stock para reactivar esta venta";
                        return RedirectToAction(nameof(Index));
                    }

                    venta.Producto.Cantidad -= venta.Cantidad;
                    _context.Update(venta.Producto);
                }

                // Para servicios, validar disponibilidad si es necesario

                TempData["SuccessMessage"] = "Venta reactivada correctamente";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
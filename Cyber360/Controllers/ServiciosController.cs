using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cyber360.Models;
using Microsoft.Data.SqlClient;

namespace Cyber360.Controllers
{
    public class ServiciosController : Controller
    {
        private readonly ProyectoCyberContext _context;

        public ServiciosController(ProyectoCyberContext context)
        {
            _context = context;
        }

        // GET: Servicios
        public async Task<IActionResult> Index()
        {
            var servicios = await _context.Servicios
                .Include(p => p.Categoria)  // Carga la relación
                .ToListAsync();

            return View(servicios);
        }

        // GET: Servicios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicio = await _context.Servicios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (servicio == null)
            {
                return NotFound();
            }

            return View(servicio);
        }

        // GET: Servicios/Create
        public IActionResult Create()
        {
            ViewBag.Categorias = new SelectList(_context.CatServis, "Id", "Nombre");
            return View();
        }

        // POST: Servicios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Precio,Detalles,CategoriaId")] Servicio servicio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(servicio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(servicio);
        }

        // GET: Servicios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio == null)
            {
                return NotFound();
            }
            return View(servicio);
        }

        // POST: Servicios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Precio,Detalles")] Servicio servicio)
        {
            if (id != servicio.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(servicio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServicioExists(servicio.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(servicio);
        }

        private bool ServicioExists(int id)
        {
            return _context.Servicios.Any(e => e.Id == id);
        }

        // GET: Servicios/Delete/5
        // GET: Servicios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicio = await _context.Servicios
                .Include(s => s.Venta) // Incluir relaciones para verificar dependencias
                .FirstOrDefaultAsync(m => m.Id == id);

            if (servicio == null)
            {
                return NotFound();
            }

            // Verificar si hay ventas asociadas
            if (servicio.Venta?.Any() == true)
            {
                ViewBag.ErrorMessage = "No se puede eliminar este servicio porque tiene ventas asociadas.";
            }

            return View(servicio);
        }

        // POST: Servicios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var servicio = await _context.Servicios
                .Include(s => s.Venta) // Incluir relaciones para verificar dependencias
                .FirstOrDefaultAsync(m => m.Id == id);

            if (servicio == null)
            {
                return NotFound();
            }

            try
            {
                // Verificar dependencias antes de eliminar
                if (servicio.Venta?.Any() == true)
                {
                    TempData["ErrorMessage"] = "No se puede eliminar el servicio porque tiene ventas asociadas.";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                _context.Servicios.Remove(servicio);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Servicio eliminado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                // Log del error (opcional)
                // _logger.LogError(ex, "Error al eliminar servicio");

                // Verificar si es una violación de clave foránea
                if (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 547))
                {
                    TempData["ErrorMessage"] = "No se puede eliminar el servicio porque está siendo utilizado en otras partes del sistema.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Ocurrió un error inesperado al intentar eliminar el servicio.";
                }

                return RedirectToAction(nameof(Delete), new { id });
            }
            catch (Exception ex)
            {
                // Log del error (opcional)
                // _logger.LogError(ex, "Error inesperado al eliminar servicio");

                TempData["ErrorMessage"] = "Ocurrió un error inesperado al intentar eliminar el servicio.";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cyber360.Models;
using Microsoft.Data.SqlClient;

namespace Cyber360.Controllers
{
    public class ProveedoresController : Controller
    {
        private readonly ProyectoCyberContext _context;

        public ProveedoresController(ProyectoCyberContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Proveedores.ToListAsync());
        }

        // GET: Proveedores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proveedore = await _context.Proveedores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (proveedore == null)
            {
                return NotFound();
            }

            return View(proveedore);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Correo,NombredeEmpresa")] Proveedore proveedore)
        {
            // Validar si el correo ya existe
            if (await _context.Proveedores.AnyAsync(p => p.Correo == proveedore.Correo))
            {
                ModelState.AddModelError("Correo", "Este correo electrónico ya está registrado");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(proveedore);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Proveedor creado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
                    {
                        ModelState.AddModelError("Correo", "Este correo electrónico ya está registrado");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Ocurrió un error al guardar el proveedor");
                    }
                }
            }
            return View(proveedore);
        }

        // GET: Proveedores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proveedore = await _context.Proveedores.FindAsync(id);
            if (proveedore == null)
            {
                return NotFound();
            }
            return View(proveedore);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Correo,NombredeEmpresa")] Proveedore proveedore)
        {
            if (id != proveedore.Id)
            {
                return NotFound();
            }

            // Validar si el correo ya existe en otro proveedor
            if (await _context.Proveedores.AnyAsync(p => p.Correo == proveedore.Correo && p.Id != proveedore.Id))
            {
                ModelState.AddModelError("Correo", "Este correo electrónico ya está registrado");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(proveedore);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Proveedor actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProveedoreExists(proveedore.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
                    {
                        ModelState.AddModelError("Correo", "Este correo electrónico ya está registrado");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Ocurrió un error al actualizar el proveedor");
                    }
                }
            }
            return View(proveedore);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proveedore = await _context.Proveedores
                .Include(s => s.Compras) // Incluir relaciones para verificar dependencias
                .FirstOrDefaultAsync(m => m.Id == id);

            if (proveedore == null)
            {
                return NotFound();
            }

            // Verificar si hay ventas asociadas
            if (proveedore.Compras?.Any() == true)
            {
                ViewBag.ErrorMessage = "No se puede eliminar este Producto porque tiene ventas asociadas.";
            }

            return View(proveedore);
        }

        // POST: Servicios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var proveedore = await _context.Proveedores
                .Include(s => s.Compras) // Incluir relaciones para verificar dependencias
                .FirstOrDefaultAsync(m => m.Id == id);

            if (proveedore == null)
            {
                return NotFound();
            }

            try
            {
                // Verificar dependencias antes de eliminar
                if (proveedore.Compras?.Any() == true)
                {
                    TempData["ErrorMessage"] = "No se puede eliminar el Producto porque tiene ventas asociadas.";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                _context.Proveedores.Remove(proveedore);
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
                    TempData["ErrorMessage"] = "No se puede eliminar el Producto porque está siendo utilizado en otras partes del sistema.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Ocurrió un error inesperado al intentar eliminar el Producto.";
                }

                return RedirectToAction(nameof(Delete), new { id });
            }
            catch (Exception ex)
            {
                // Log del error (opcional)
                // _logger.LogError(ex, "Error inesperado al eliminar servicio");

                TempData["ErrorMessage"] = "Ocurrió un error inesperado al intentar eliminar el Producto.";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }


        private bool ProveedoreExists(int id)
        {
            return _context.Proveedores.Any(e => e.Id == id);
        }
    }
}
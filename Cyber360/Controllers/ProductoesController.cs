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
    public class ProductoesController : Controller
    {
        private readonly ProyectoCyberContext _context;

        public ProductoesController(ProyectoCyberContext context)
        {
            _context = context;
        }

        // GET: Productoes
        public async Task<IActionResult> Index()
        {
            var productos = await _context.Productos
                .Include(p => p.Categoria)  // Carga la relación
                .ToListAsync();

            return View(productos);
        }

        // GET: Productoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productoes/Create
        // GET: Productos/Create
        public IActionResult Create()
        {
            ViewBag.Categorias = new SelectList(_context.CatProducts, "Id", "Nombre");
            return View();
        }

        // POST: Productos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Precio,Cantidad,CategoriaId")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categorias = new SelectList(_context.CatProducts, "Id", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        // GET: Productoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }

        // POST: Productoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Precio,Cantidad,CategoriaId")] Producto producto)
        {
            if (id != producto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();

                    // Redirigir al Index con mensaje de éxito
                    TempData["SuccessMessage"] = "Producto actualizado correctamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Si hay errores de validación, recargar las categorías y mostrar el formulario nuevamente
            ViewBag.Categorias = new SelectList(_context.Productos, "Id", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }

        // GET: Productoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicio = await _context.Productos
                .Include(s => s.Venta) // Incluir relaciones para verificar dependencias
                .FirstOrDefaultAsync(m => m.Id == id);

            if (servicio == null)
            {
                return NotFound();
            }

            // Verificar si hay ventas asociadas
            if (servicio.Venta?.Any() == true)
            {
                ViewBag.ErrorMessage = "No se puede eliminar este Producto porque tiene ventas asociadas.";
            }

            return View(servicio);
        }

        // POST: Servicios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos
                .Include(s => s.Venta) // Incluir relaciones para verificar dependencias
                .FirstOrDefaultAsync(m => m.Id == id);

            if (producto == null)
            {
                return NotFound();
            }

            try
            {
                // Verificar dependencias antes de eliminar
                if (producto.Venta?.Any() == true)
                {
                    TempData["ErrorMessage"] = "No se puede eliminar el Producto porque tiene ventas asociadas.";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                _context.Productos.Remove(producto);
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
    }
}

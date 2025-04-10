using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cyber360.Models;
using Cyber360.Models.viewModel;
using Microsoft.Data.SqlClient;

namespace Cyber360.Controllers
{
    public class CatProductsController : Controller
    {
        private readonly ProyectoCyberContext _context;

        public CatProductsController(ProyectoCyberContext context)
        {
            _context = context;
        }

        // GET: CatProducts
        public async Task<IActionResult> Index()
        {
            // Obtener categorías con conteo de productos
            var categoriasConConteo = await _context.CatProducts
                .Select(c => new CategoriaConConteoViewModel
                {
                    Categoria = c,
                    TotalProductos = c.Productos.Count
                })
                .ToListAsync();

            return View(categoriasConConteo);
        }

        // GET: CatProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catProduct = await _context.CatProducts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (catProduct == null)
            {
                return NotFound();
            }

            return View(catProduct);
        }

        // GET: CatProducts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CatProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre")] CatProduct catProduct)
        {
            if (ModelState.IsValid)
            {
                _context.Add(catProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(catProduct);
        }

        // GET: CatProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catProduct = await _context.CatProducts.FindAsync(id);
            if (catProduct == null)
            {
                return NotFound();
            }
            return View(catProduct);
        }

        // POST: CatProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre")] CatProduct catProduct)
        {
            if (id != catProduct.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(catProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CatProductExists(catProduct.Id))
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
            return View(catProduct);
        }

        // GET: CatProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || !CatProductExists(id.Value))
            {
                return NotFound();
            }

            var catProduct = await _context.CatProducts
                .Include(c => c.Productos) // Incluir servicios relacionados
                .FirstOrDefaultAsync(m => m.Id == id);

            if (catProduct == null)
            {
                return NotFound();
            }

            // Verificar si hay servicios asociados
            if (catProduct.Productos?.Any() == true)
            {
                ViewBag.ErrorMessage = "No se puede eliminar esta categoría porque tiene Productos asociados.";
            }

            return View(catProduct);
        }

        // POST: CatServis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!CatProductExists(id))
            {
                return NotFound();
            }

            var catProduct = await _context.CatProducts
                .Include(c => c.Productos)
                .FirstOrDefaultAsync(m => m.Id == id);

            try
            {
                // Verificar dependencias antes de eliminar
                if (catProduct.Productos?.Any() == true)
                {
                    TempData["ErrorMessage"] = "No se puede eliminar la categoría porque tiene Productos asociados.";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                _context.CatProducts.Remove(catProduct);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Categoría eliminada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                // Verificar si es una violación de clave foránea
                if (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 547))
                {
                    TempData["ErrorMessage"] = "No se puede eliminar la categoría porque está siendo utilizada en otros registros.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Ocurrió un error al intentar eliminar la categoría.";
                }

                return RedirectToAction(nameof(Delete), new { id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al intentar eliminar la categoría.";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        private bool CatProductExists(int id)
        {
            return _context.CatProducts.Any(e => e.Id == id);
        }
    }
}

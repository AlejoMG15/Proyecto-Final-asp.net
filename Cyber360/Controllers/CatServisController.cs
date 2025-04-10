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
    public class CatServisController : Controller
    {
        private readonly ProyectoCyberContext _context;

        public CatServisController(ProyectoCyberContext context)
        {
            _context = context;
        }

        // GET: CatServis
        public async Task<IActionResult> Index()
        {
            // Obtener categorías con conteo de productos
            var categoriasConConteo = await _context.CatServis
                .Select(c => new CategoriaConConteoViewModels
                {
                    Categoria = c,
                    TotalServicios = c.Servicios.Count
                })
                .ToListAsync();

            return View(categoriasConConteo);
        }

        // GET: CatServis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catServi = await _context.CatServis
                .FirstOrDefaultAsync(m => m.Id == id);
            if (catServi == null)
            {
                return NotFound();
            }

            return View(catServi);
        }

        // GET: CatServis/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CatServis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre")] CatServi catServi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(catServi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(catServi);
        }

        // GET: CatServis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catServi = await _context.CatServis.FindAsync(id);
            if (catServi == null)
            {
                return NotFound();
            }
            return View(catServi);
        }

        // POST: CatServis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre")] CatServi catServi)
        {
            if (id != catServi.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(catServi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CatServiExists(catServi.Id))
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
            return View(catServi);
        }

        // GET: CatServis/Delete/5
        // GET: CatServis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || !CatServiExists(id.Value))
            {
                return NotFound();
            }

            var catServi = await _context.CatServis
                .Include(c => c.Servicios) // Incluir servicios relacionados
                .FirstOrDefaultAsync(m => m.Id == id);

            if (catServi == null)
            {
                return NotFound();
            }

            // Verificar si hay servicios asociados
            if (catServi.Servicios?.Any() == true)
            {
                ViewBag.ErrorMessage = "No se puede eliminar esta categoría porque tiene servicios asociados.";
            }

            return View(catServi);
        }

        // POST: CatServis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!CatServiExists(id))
            {
                return NotFound();
            }

            var catServi = await _context.CatServis
                .Include(c => c.Servicios)
                .FirstOrDefaultAsync(m => m.Id == id);

            try
            {
                // Verificar dependencias antes de eliminar
                if (catServi.Servicios?.Any() == true)
                {
                    TempData["ErrorMessage"] = "No se puede eliminar la categoría porque tiene servicios asociados.";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                _context.CatServis.Remove(catServi);
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

        private bool CatServiExists(int id)
        {
            return _context.CatServis.Any(e => e.Id == id);
        }
    }
}

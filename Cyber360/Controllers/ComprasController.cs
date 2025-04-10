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
    public class ComprasController : Controller
    {
        private readonly ProyectoCyberContext _context;

        public ComprasController(ProyectoCyberContext context)
        {
            _context = context;
        }

        // GET: Compras
        public async Task<IActionResult> Index()
        {
            var proyectoCyberContext = _context.Compras.Include(c => c.Proveedor);
            return View(await proyectoCyberContext.ToListAsync());
        }

        // GET: Compras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compra = await _context.Compras
                .Include(c => c.Proveedor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (compra == null)
            {
                return NotFound();
            }

            return View(compra);
        }

        // GET: Compras/Create
        public IActionResult Create()
        {
            // Cambiado para mostrar un campo descriptivo en lugar del ID
            ViewData["ProveedorId"] = new SelectList(_context.Proveedores, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProveedorId,Cantidad,Total,Fecha")] Compra compra)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Validación adicional para asegurar datos correctos
                    if (compra.Cantidad <= 0)
                    {
                        ModelState.AddModelError("Cantidad", "La cantidad debe ser mayor que cero");
                        ViewData["ProveedorId"] = new SelectList(_context.Proveedores, "Id", "Nombre", compra.ProveedorId);
                        return View(compra);
                    }

                    if (compra.Total <= 0)
                    {
                        ModelState.AddModelError("Total", "El total debe ser mayor que cero");
                        ViewData["ProveedorId"] = new SelectList(_context.Proveedores, "Id", "Nombre", compra.ProveedorId);
                        return View(compra);
                    }

                    // Asignar fecha actual si no se proporcionó
                    if (compra.Fecha == null)
                    {
                        compra.Fecha = DateTime.Now;
                    }

                    _context.Add(compra);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Compra registrada exitosamente";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException ex)
            {
                // Log del error (implementar según tu sistema de logging)
                ModelState.AddModelError("", "Error al guardar en la base de datos: " + ex.InnerException?.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error inesperado: " + ex.Message);
            }

            // Recargar los proveedores si hay error
            ViewData["ProveedorId"] = new SelectList(_context.Proveedores, "Id", "Nombre", compra.ProveedorId);
            return View(compra);
        }
        // GET: Compras/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var compra = await _context.Compras.FindAsync(id);
        //    if (compra == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["ProveedorId"] = new SelectList(_context.Proveedores, "Id", "Id", compra.ProveedorId);
        //    return View(compra);
        //}

        //// POST: Compras/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,ProveedorId,Cantidad,Total,Fecha")] Compra compra)
        //{
        //    if (id != compra.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(compra);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!CompraExists(compra.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["ProveedorId"] = new SelectList(_context.Proveedores, "Id", "Id", compra.ProveedorId);
        //    return View(compra);
        //}

        // GET: Compras/Delete/5
//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var compra = await _context.Compras
//                .Include(c => c.Proveedor)
//                .FirstOrDefaultAsync(m => m.Id == id);
//            if (compra == null)
//            {
//                return NotFound();
//            }

//            return View(compra);
//        }

//        // POST: Compras/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var compra = await _context.Compras.FindAsync(id);
//            if (compra != null)
//            {
//                _context.Compras.Remove(compra);
//            }

//            await _context.SaveChangesAsync();
//            return RedirectToAction(nameof(Index));
//        }

//        private bool CompraExists(int id)
//        {
//            return _context.Compras.Any(e => e.Id == id);
//        }
      }
}

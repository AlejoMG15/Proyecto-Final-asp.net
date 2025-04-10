using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cyber360.Models;
using Microsoft.Data.SqlClient;

namespace Cyber360.Controllers
{
    public class ClientesController : Controller
    {
        private readonly ProyectoCyberContext _context;

        public ClientesController(ProyectoCyberContext context)
        {
            _context = context;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clientes.ToListAsync());
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,Telefono,Documento")] Cliente cliente)
        {
            // Validar si el documento ya existe
            if (await _context.Clientes.AnyAsync(c => c.Documento == cliente.Documento))
            {
                ModelState.AddModelError("Documento", "Este número de documento ya está registrado");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(cliente);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cliente creado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
                    {
                        ModelState.AddModelError("Documento", "Este número de documento ya está registrado");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Ocurrió un error al guardar el cliente");
                    }
                }
            }
            return View(cliente);
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Telefono,Documento")] Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return NotFound();
            }

            // Validar si el documento ya existe en otro cliente
            if (await _context.Clientes.AnyAsync(c => c.Documento == cliente.Documento && c.Id != cliente.Id))
            {
                ModelState.AddModelError("Documento", "Este número de documento ya está registrado");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cliente actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
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
                        ModelState.AddModelError("Documento", "Este número de documento ya está registrado");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Ocurrió un error al actualizar el cliente");
                    }
                }
            }
            return View(cliente);
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .Include(c => c.Venta)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            if (cliente.Venta?.Any() == true)
            {
                ViewBag.ErrorMessage = "No se puede eliminar este cliente porque tiene ventas asociadas.";
            }

            return View(cliente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Venta)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            try
            {
                if (cliente.Venta?.Any() == true)
                {
                    TempData["ErrorMessage"] = "No se puede eliminar el cliente porque tiene ventas asociadas.";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cliente eliminado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
                {
                    TempData["ErrorMessage"] = "No se puede eliminar el cliente porque está siendo utilizado en otras partes del sistema.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Ocurrió un error inesperado al intentar eliminar el cliente.";
                }
                return RedirectToAction(nameof(Delete), new { id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al intentar eliminar el cliente.";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }
    }
}
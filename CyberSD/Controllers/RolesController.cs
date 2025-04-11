using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CyberSD.Models; // Asegúrate de tener este namespace
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Roles = "Administrador")]
public class RolesController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager; // Añadir esto

    public RolesController(
        RoleManager<IdentityRole> roleManager,
        UserManager<IdentityUser> userManager) // Modificar constructor
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    // GET: Lista todos los roles
    public IActionResult Index()
    {
        var roles = _roleManager.Roles.ToList();
        return View(roles);
    }

    // GET: Muestra formulario para crear rol
    public IActionResult Create()
    {
        return View();
    }

    // POST: Crea nuevo rol
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name")] CreateRoleViewModel model)
    {
        if (ModelState.IsValid)
        {
            var roleExists = await _roleManager.RoleExistsAsync(model.Name);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole(model.Name));
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "El rol ya existe");
        }
        return View(model);
    }

    // GET: Muestra usuarios con este rol
    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();

        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
            return NotFound();

        // Obtener todos los usuarios con este rol (usando UserManager)
        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);

        var model = new RoleDetailsViewModel
        {
            RoleId = role.Id,
            RoleName = role.Name,
            Users = usersInRole.Select(u => u.Email).ToList() // Lista de emails
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRole(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
        {
            return NotFound();
        }

        // Prevenir eliminación de roles esenciales
        if (role.Name == "Administrador" || role.Name == "Usuario")
        {
            TempData["ErrorMessage"] = $"No se puede eliminar el rol {role.Name} porque es esencial";
            return RedirectToAction(nameof(Index));
        }

        // Verificar si hay usuarios con este rol
        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
        if (usersInRole.Any())
        {
            TempData["ErrorMessage"] = $"No se puede eliminar el rol {role.Name} porque tiene {usersInRole.Count} usuarios asignados";
            return RedirectToAction(nameof(Index));
        }

        var result = await _roleManager.DeleteAsync(role);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = $"Rol {role.Name} eliminado correctamente";
        }
        else
        {
            TempData["ErrorMessage"] = $"Error al eliminar el rol: {string.Join(", ", result.Errors.Select(e => e.Description))}";
        }

        return RedirectToAction(nameof(Index));
    }

}
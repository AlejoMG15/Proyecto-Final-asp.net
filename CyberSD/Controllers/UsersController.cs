using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CyberSD.Models; // Asegúrate de tener este namespace
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Administrador")]

public class UsersController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // GET: Listar todos los usuarios CON SUS ROLES
    public async Task<IActionResult> Index()
    {
        var users = _userManager.Users.ToList();
        var usersWithRoles = new List<UserWithRolesViewModel>();

        foreach (var user in users)
        {
            usersWithRoles.Add(new UserWithRolesViewModel
            {
                User = user,
                Roles = await _userManager.GetRolesAsync(user)
            });
        }

        return View(usersWithRoles);
    }

    // GET: Editar usuario (MANTENIDO para compatibilidad)
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var userRoles = await _userManager.GetRolesAsync(user);
        var allRoles = _roleManager.Roles.ToList();

        ViewBag.AllRoles = allRoles.Select(r => new SelectListItem
        {
            Text = r.Name,
            Value = r.Name
        }).ToList();

        ViewBag.UserRoles = userRoles;

        return View(user);
    }

    // POST: Actualizar usuario y roles (MANTENIDO)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, List<string> roles)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var currentRoles = await _userManager.GetRolesAsync(user);

        // Optimización: Solo actualiza si hay cambios
        if (!currentRoles.SequenceEqual(roles))
        {
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRolesAsync(user, roles);
        }

        return RedirectToAction(nameof(Index));
    }
    // En el método ManageRoles GET:
    public async Task<IActionResult> ManageRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var userRoles = await _userManager.GetRolesAsync(user);
        var allRoles = await _roleManager.Roles.ToListAsync();

        var model = new ManageRolesViewModel
        {
            UserId = user.Id,
            UserEmail = user.Email,
            AvailableRoles = allRoles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name,
                Selected = userRoles.Contains(r.Name)
            }).ToList(),
            SelectedRolesNames = userRoles.ToList() // Cambiado a SelectedRolesNames
        };

        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> ManageRoles(ManageRolesViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null) return NotFound();

        // Usa SelectedRolesNames aquí
        await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
        await _userManager.AddToRolesAsync(user, model.SelectedRolesNames);

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        // Evitar que un usuario se elimine a sí mismo
        if (user.Id == _userManager.GetUserId(User))
        {
            TempData["ErrorMessage"] = "No puedes eliminarte a ti mismo";
            return RedirectToAction(nameof(Index));
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Usuario eliminado correctamente";
        }
        else
        {
            TempData["ErrorMessage"] = "Error al eliminar el usuario";
        }

        return RedirectToAction(nameof(Index));
    }
}
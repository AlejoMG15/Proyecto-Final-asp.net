using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace CyberSD.Models
{
    public class ManageRolesViewModel
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }

        // Cambia a SelectedRolesNames para coincidir con tu código
        public List<string> SelectedRolesNames { get; set; } = new();

        // Lista de roles disponibles
        public List<SelectListItem> AvailableRoles { get; set; } = new();
    }
}
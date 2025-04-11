using System.ComponentModel.DataAnnotations;

namespace CyberSD.Models
{
    public class CreateRoleViewModel
    {
        [Required]
        [Display(Name = "Nombre del Rol")]
        public string Name { get; set; }
    }
}
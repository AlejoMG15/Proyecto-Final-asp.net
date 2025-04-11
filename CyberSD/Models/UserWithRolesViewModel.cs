// Models/UserWithRolesViewModel.cs
using Microsoft.AspNetCore.Identity;

namespace CyberSD.Models
{
    public class UserWithRolesViewModel
    {
        public IdentityUser User { get; set; }
        public IList<string> Roles { get; set; }
    }
}
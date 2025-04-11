using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace CyberSD.Helpers  // Asegúrate de usar tu namespace correcto
{
    public class DynamicRoleAuthorizationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            // 1. Definir roles permitidos (puedes cargarlos desde DB)
            var allowedRoles = new[] { "Administrador", "Empleado", "Gestor_Equipos" };

            // 2. Verificar si el usuario tiene alguno de los roles
            var user = context.HttpContext.User;
            var hasAccess = allowedRoles.Any(role => user.IsInRole(role));

            // 3. Denegar acceso si no tiene permisos
            if (!hasAccess)
            {
                context.Result = new ForbidResult();
                return;
            }

            // 4. Continuar con la ejecución si está autorizado
            await next();
        }
    }
}
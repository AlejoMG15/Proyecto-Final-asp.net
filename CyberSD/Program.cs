using CyberSD.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CyberSD.Helpers;

namespace CyberSD
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //Validaci?n de la cadena de conexion
            builder.Services.AddDbContext<CyberSdContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("Conexion")));

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<CyberSdContext>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout"; // Asegúrate que esto esté configurado
                options.SlidingExpiration = true;
            });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "TuCookieDeSesion";
                options.LoginPath = "/Identity/Account/Login"; // Asegúrate que sea la ruta correcta
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.None; // Usa "Always" si tienes HTTPS
            });
            

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login"; // Ruta correcta para Identity
                                                               // O si usas una ruta personalizada:
                                                               // options.LoginPath = "/Cuentas/IniciarSesion";
            });
            builder.Services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.FromSeconds(10);
            });
            builder.Services.AddAuthorization(options =>
            {
                // Política que permite a Gestores, Administradores O Empleados
                options.AddPolicy("RequireGestorOrAdminOrEmpleado", policy =>
                    policy.RequireRole("Gestor_Equipos", "Administrador", "Empleado"));

                // Otras políticas si las necesitas
            });
            builder.Services.AddScoped<DynamicRoleAuthorizationFilter>();

            // Opcional: Para aplicar globalmente
            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<DynamicRoleAuthorizationFilter>();
            });



            var app = builder.Build();



            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "roles",
                pattern: "Roles/{action=Index}/{id?}");
            
            app.MapRazorPages();

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                string[] roles = { "Administrador", "Gestor_Equipos", "Empleado", "Rol_Eliminable" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }   
            }

            app.Run();
        }
    }
}

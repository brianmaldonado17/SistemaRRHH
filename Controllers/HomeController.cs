using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaRRHH.Models;
using System.Diagnostics;
using Dapper;
using SistemaRRHH.Datos;

namespace SistemaRRHH.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ConexionDb _conexion;

        public HomeController(ILogger<HomeController> logger, ConexionDb conexion)
        {
            _logger = logger;
            _conexion = conexion;
        }

        public async Task<IActionResult> Index()
        {
            using (var db = _conexion.ObtenerConexion())
            {
                // ==========================================
                // DASHBOARD: ADMINISTRADOR (TI / Sistemas)
                // ==========================================
                if (User.IsInRole("Administrador"))
                {
                    ViewBag.TotalUsuarios = await db.QueryFirstOrDefaultAsync<int>("SELECT COUNT(*) FROM usuarios WHERE estado = 'Activo'");
                    ViewBag.TotalRoles = await db.QueryFirstOrDefaultAsync<int>("SELECT COUNT(*) FROM roles WHERE estado = 'Activo'");
                    ViewBag.TotalDeptos = await db.QueryFirstOrDefaultAsync<int>("SELECT COUNT(*) FROM departamentos WHERE estado = 'Activo'");
                }

                // ==========================================
                // DASHBOARD: RECURSOS HUMANOS
                // ==========================================
                else if (User.IsInRole("RecursosHumanos"))
                {
                    ViewBag.TotalEmpleados = await db.QueryFirstOrDefaultAsync<int>("SELECT COUNT(*) FROM empleados WHERE estado = 'Activo'");
                    ViewBag.NominasPendientes = await db.QueryFirstOrDefaultAsync<int>("SELECT COUNT(*) FROM nominas WHERE estado NOT IN ('Pagada', 'Anulada')");
                    ViewBag.PrestacionesPendientes = await db.QueryFirstOrDefaultAsync<int>("SELECT COUNT(*) FROM historial_prestaciones WHERE estado = 'Pendiente'");
                }

                // ==========================================
                // DASHBOARD: COLABORADOR
                // ==========================================
                else if (User.IsInRole("Colaborador"))
                {
                    string username = User.Identity.Name;

                    var perfil = await db.QueryFirstOrDefaultAsync<dynamic>(@"
                        SELECT 
                            e.nombre AS Nombre, 
                            e.apellido AS Apellido, 
                            e.fecha_ingreso AS FechaIngreso, 
                            p.nombre_puesto AS NombrePuesto, 
                            d.nombre_departamento AS NombreDepartamento
                        FROM empleados e
                        INNER JOIN usuarios u ON e.id_empleado = u.id_empleado
                        INNER JOIN puestos p ON e.id_puesto = p.id_puesto
                        INNER JOIN departamentos d ON e.id_departamento = d.id_departamento
                        WHERE u.username = @User", new { User = username });

                    ViewBag.Perfil = perfil;

                    // NUEVO: Consultar cantidad de ausencias del año actual
                    ViewBag.AusenciasAnio = await db.QueryFirstOrDefaultAsync<int>(@"
                        SELECT COUNT(*) 
                        FROM ausencias a
                        INNER JOIN usuarios u ON a.id_empleado = u.id_empleado
                        WHERE u.username = @User 
                          AND a.estado = 'Activo' 
                          AND YEAR(a.fecha_ausencia) = YEAR(CURDATE())",
                        new { User = username });
                }
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
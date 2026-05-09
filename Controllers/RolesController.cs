using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Dapper;
using SistemaRRHH.Datos;
using SistemaRRHH.Models;

namespace SistemaRRHH.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class RolesController : Controller
    {
        private readonly ConexionDb _conexion;

        public RolesController(ConexionDb conexion)
        {
            _conexion = conexion;
        }

        public async Task<IActionResult> Index()
        {
            using (var db = _conexion.ObtenerConexion())
            {
                // CORRECCIÓN: Agregamos estado AS Estado
                var sql = "SELECT id_rol AS IdRol, nombre_rol AS NombreRol, estado AS Estado FROM roles";
                var lista = await db.QueryAsync<Rol>(sql);
                return View(lista);
            }
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Rol modelo)
        {
            if (ModelState.IsValid)
            {
                using (var db = _conexion.ObtenerConexion())
                {
                    var sql = "INSERT INTO roles (nombre_rol) VALUES (@NombreRol)";
                    await db.ExecuteAsync(sql, modelo);

                    TempData["Exito"] = "Rol creado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                var rol = await db.QueryFirstOrDefaultAsync<dynamic>("SELECT id_rol AS IdRol, nombre_rol AS NombreRol, estado AS Estado FROM roles WHERE id_rol = @Id", new { Id = id });
                return View(rol);
            }
        }

        // Acción para guardar los cambios (AHORA SOLO ACTUALIZA EL NOMBRE)
        [HttpPost]
        public async Task<IActionResult> Editar(int IdRol, string NombreRol)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                await db.ExecuteAsync("UPDATE roles SET nombre_rol = @Nombre WHERE id_rol = @Id", new { Nombre = NombreRol, Id = IdRol });

                TempData["Exito"] = "Rol actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // Acción para la anulación lógica
        [HttpPost]
        public async Task<IActionResult> Anular(int id)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                // CANDADO DE INTEGRIDAD: Validamos si hay usuarios activos con este rol
                var sqlValidacion = "SELECT COUNT(1) FROM usuarios WHERE id_rol = @Id AND estado = 'Activo'";
                var usuariosActivos = await db.QuerySingleOrDefaultAsync<int>(sqlValidacion, new { Id = id });

                if (usuariosActivos > 0)
                {
                    TempData["Error"] = $"No se puede anular este rol porque hay {usuariosActivos} usuario(s) activo(s) utilizándolo. Cámbieles el rol primero.";
                    return RedirectToAction(nameof(Index));
                }

                await db.ExecuteAsync("UPDATE roles SET estado = 'Inactivo' WHERE id_rol = @Id", new { Id = id });

                TempData["Exito"] = "Rol inactivado correctamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // NUEVA ACCIÓN: Para revivir un rol
        [HttpPost]
        public async Task<IActionResult> Activar(int id)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                await db.ExecuteAsync("UPDATE roles SET estado = 'Activo' WHERE id_rol = @Id", new { Id = id });

                TempData["Exito"] = "Rol reactivado correctamente.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
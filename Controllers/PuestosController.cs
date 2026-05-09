using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Dapper;
using SistemaRRHH.Datos;
using SistemaRRHH.Models;

namespace SistemaRRHH.Controllers
{
    [Authorize(Roles = "Administrador,RecursosHumanos")]
    public class PuestosController : Controller
    {
        private readonly ConexionDb _conexion;

        public PuestosController(ConexionDb conexion)
        {
            _conexion = conexion;
        }

        // Listar los puestos
        public async Task<IActionResult> Index()
        {
            using (var db = _conexion.ObtenerConexion())
            {
                // CORRECCIÓN: Agregamos estado AS Estado
                var sql = "SELECT id_puesto AS IdPuesto, nombre_puesto AS NombrePuesto, salario_base AS SalarioBase, estado AS Estado FROM puestos";
                var lista = await db.QueryAsync<Puesto>(sql);

                return View(lista);
            }
        }

        // Mostrar la pantalla de creación
        public IActionResult Crear()
        {
            return View();
        }

        // Recibir los datos del formulario y meterlos a MySQL
        [HttpPost]
        public async Task<IActionResult> Crear(Puesto modelo)
        {
            if (ModelState.IsValid)
            {
                using (var db = _conexion.ObtenerConexion())
                {
                    var sql = "INSERT INTO puestos (nombre_puesto, salario_base) VALUES (@NombrePuesto, @SalarioBase)";
                    await db.ExecuteAsync(sql, modelo);

                    TempData["Exito"] = "Puesto creado correctamente.";
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
                var sql = "SELECT id_puesto AS IdPuesto, nombre_puesto AS NombrePuesto, salario_base AS SalarioBase, estado AS Estado FROM puestos WHERE id_puesto = @Id";
                var puesto = await db.QueryFirstOrDefaultAsync<dynamic>(sql, new { Id = id });
                return View(puesto);
            }
        }

        // Acción para guardar los cambios (AHORA SOLO ACTUALIZA NOMBRE Y SALARIO)
        [HttpPost]
        public async Task<IActionResult> Editar(int IdPuesto, string NombrePuesto, decimal SalarioBase)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                var sql = "UPDATE puestos SET nombre_puesto = @Nombre, salario_base = @Salario WHERE id_puesto = @Id";
                await db.ExecuteAsync(sql, new { Nombre = NombrePuesto, Salario = SalarioBase, Id = IdPuesto });

                TempData["Exito"] = "Puesto actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // Acción para la anulación lógica
        [HttpPost]
        public async Task<IActionResult> Anular(int id)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                // CANDADO DE INTEGRIDAD
                var sqlValidacion = "SELECT COUNT(1) FROM empleados WHERE id_puesto = @Id AND estado = 'Activo'";
                var empleadosActivos = await db.QuerySingleOrDefaultAsync<int>(sqlValidacion, new { Id = id });

                if (empleadosActivos > 0)
                {
                    TempData["Error"] = $"No se puede anular el puesto porque tiene {empleadosActivos} empleado(s) activo(s) asignado(s). Trasládelos primero.";
                    return RedirectToAction(nameof(Index));
                }

                await db.ExecuteAsync("UPDATE puestos SET estado = 'Inactivo' WHERE id_puesto = @Id", new { Id = id });

                TempData["Exito"] = "Puesto inactivado correctamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // NUEVA ACCIÓN: Para revivir un puesto
        [HttpPost]
        public async Task<IActionResult> Activar(int id)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                await db.ExecuteAsync("UPDATE puestos SET estado = 'Activo' WHERE id_puesto = @Id", new { Id = id });

                TempData["Exito"] = "Puesto reactivado correctamente.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
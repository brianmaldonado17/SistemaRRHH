using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Dapper;
using SistemaRRHH.Datos;
using SistemaRRHH.Models;

namespace SistemaRRHH.Controllers
{
    [Authorize(Roles = "Administrador,RecursosHumanos")]
    public class DepartamentosController : Controller
    {
        private readonly ConexionDb _conexion;

        public DepartamentosController(ConexionDb conexion)
        {
            _conexion = conexion;
        }

        // Listar los departamentos
        public async Task<IActionResult> Index()
        {
            using (var db = _conexion.ObtenerConexion())
            {
                var sql = "SELECT id_departamento AS IdDepartamento, nombre_departamento AS NombreDepartamento, estado AS Estado FROM departamentos";
                var lista = await db.QueryAsync<Departamento>(sql);

                return View(lista);
            }
        }

        // Mostrar la pantalla de creación
        public IActionResult Crear()
        {
            return View();
        }

        //Insert
        [HttpPost]
        public async Task<IActionResult> Crear(Departamento modelo)
        {
            if (ModelState.IsValid)
            {
                using (var db = _conexion.ObtenerConexion())
                {
                    var sql = "INSERT INTO departamentos (nombre_departamento) VALUES (@NombreDepartamento)";
                    await db.ExecuteAsync(sql, modelo);

                    TempData["Exito"] = "Departamento creado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(modelo);
        }

        // Acción para mostrar el formulario de edición
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                var sql = "SELECT id_departamento AS IdDepartamento, nombre_departamento AS NombreDepartamento, estado AS Estado FROM departamentos WHERE id_departamento = @Id";
                var departamento = await db.QueryFirstOrDefaultAsync<dynamic>(sql, new { Id = id });

                if (departamento == null) return NotFound();
                return View(departamento);
            }
        }

        // Acción para guardar los cambios (AHORA SOLO ACTUALIZA EL NOMBRE)
        [HttpPost]
        public async Task<IActionResult> Editar(int IdDepartamento, string NombreDepartamento)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                var sql = "UPDATE departamentos SET nombre_departamento = @Nombre WHERE id_departamento = @Id";
                await db.ExecuteAsync(sql, new { Nombre = NombreDepartamento, Id = IdDepartamento });

                TempData["Exito"] = "Departamento actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // Acción para la anulación lógica (Borrado seguro)
        [HttpPost]
        public async Task<IActionResult> Anular(int id)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                // CANDADO DE INTEGRIDAD
                var sqlValidacion = "SELECT COUNT(1) FROM empleados WHERE id_departamento = @Id AND estado = 'Activo'";
                var empleadosActivos = await db.QuerySingleOrDefaultAsync<int>(sqlValidacion, new { Id = id });

                if (empleadosActivos > 0)
                {
                    TempData["Error"] = $"No se puede anular el departamento porque tiene {empleadosActivos} empleado(s) activo(s) asignado(s). Trasládelos primero.";
                    return RedirectToAction(nameof(Index));
                }

                await db.ExecuteAsync("UPDATE departamentos SET estado = 'Inactivo' WHERE id_departamento = @Id", new { Id = id });

                TempData["Exito"] = "Departamento inactivado correctamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // NUEVA ACCIÓN: Para revivir un registro inactivado
        [HttpPost]
        public async Task<IActionResult> Activar(int id)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                await db.ExecuteAsync("UPDATE departamentos SET estado = 'Activo' WHERE id_departamento = @Id", new { Id = id });

                TempData["Exito"] = "Departamento reactivado correctamente.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
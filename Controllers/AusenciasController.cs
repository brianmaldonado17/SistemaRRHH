using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Dapper;
using SistemaRRHH.Datos;
using SistemaRRHH.Models;

namespace SistemaRRHH.Controllers
{
    [Authorize(Roles = "Administrador,RecursosHumanos")]
    public class AusenciasController : Controller
    {
        private readonly ConexionDb _conexion;

        public AusenciasController(ConexionDb conexion)
        {
            _conexion = conexion;
        }

        // 1. Mostrar historial de ausencias (CON VERIFICACIÓN DE NÓMINA)
        public async Task<IActionResult> Index()
        {
            using (var db = _conexion.ObtenerConexion())
            {
                var sql = @"
                    SELECT a.id_ausencia AS IdAusencia, a.id_empleado AS IdEmpleado, 
                           CONCAT(e.nombre, ' ', e.apellido) AS NombreEmpleado,
                           a.fecha_ausencia AS FechaAusencia, a.motivo AS Motivo, 
                           a.descuenta_salario AS DescuentaSalario,
                           a.estado AS Estado,
                           -- MAGIA: Verificamos si la fecha de ausencia cae dentro de una nómina ya generada
                           CASE WHEN EXISTS (
                               SELECT 1 FROM nominas n 
                               WHERE a.fecha_ausencia BETWEEN n.fecha_inicio AND n.fecha_fin 
                               AND n.estado != 'Anulada'
                           ) THEN 1 ELSE 0 END AS YaProcesada
                    FROM ausencias a
                    INNER JOIN empleados e ON a.id_empleado = e.id_empleado
                    ORDER BY a.fecha_ausencia DESC";

                var lista = await db.QueryAsync<Ausencia>(sql);
                return View(lista);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Registrar()
        {
            using (var db = _conexion.ObtenerConexion())
            {
                ViewBag.Empleados = await db.QueryAsync("SELECT id_empleado AS IdEmpleado, CONCAT(nombre, ' ', apellido) AS NombreCompleto FROM empleados WHERE estado = 'Activo'");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(Ausencia modelo)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                var sql = @"INSERT INTO ausencias (id_empleado, fecha_ausencia, motivo, descuenta_salario) 
                            VALUES (@IdEmpleado, @FechaAusencia, @Motivo, @DescuentaSalario)";
                await db.ExecuteAsync(sql, modelo);

                TempData["Exito"] = "Falta registrada correctamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                var ausencia = await db.QueryFirstOrDefaultAsync<Ausencia>(
                    "SELECT id_ausencia AS IdAusencia, id_empleado AS IdEmpleado, fecha_ausencia AS FechaAusencia, motivo AS Motivo, descuenta_salario AS DescuentaSalario, estado AS Estado FROM ausencias WHERE id_ausencia = @Id", new { Id = id });

                if (ausencia == null) return NotFound();

                // CANDADO BACKEND: Evitar que entren pegando el ID en la URL
                var procesada = await db.QuerySingleOrDefaultAsync<int>(
                    "SELECT COUNT(1) FROM nominas WHERE @Fecha BETWEEN fecha_inicio AND fecha_fin AND estado != 'Anulada'",
                    new { Fecha = ausencia.FechaAusencia });

                if (procesada > 0)
                {
                    TempData["Error"] = "Acceso denegado. Esta ausencia ya fue procesada en una nómina cerrada y no puede modificarse.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Empleados = await db.QueryAsync("SELECT id_empleado AS IdEmpleado, CONCAT(nombre, ' ', apellido) AS NombreCompleto FROM empleados WHERE estado = 'Activo'");
                return View(ausencia);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Ausencia modelo)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                var sql = @"UPDATE ausencias SET 
                            id_empleado = @IdEmpleado, 
                            fecha_ausencia = @FechaAusencia, 
                            motivo = @Motivo, 
                            descuenta_salario = @DescuentaSalario 
                            WHERE id_ausencia = @IdAusencia";
                await db.ExecuteAsync(sql, modelo);

                TempData["Exito"] = "Registro actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Anular(int id)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                // Obtenemos la fecha para validar
                var fecha = await db.QueryFirstOrDefaultAsync<DateTime>("SELECT fecha_ausencia FROM ausencias WHERE id_ausencia = @Id", new { Id = id });

                var procesada = await db.QuerySingleOrDefaultAsync<int>(
                    "SELECT COUNT(1) FROM nominas WHERE @Fecha BETWEEN fecha_inicio AND fecha_fin AND estado != 'Anulada'", new { Fecha = fecha });

                if (procesada > 0)
                {
                    TempData["Error"] = "No se puede anular. Esta falta ya afectó una nómina cerrada.";
                    return RedirectToAction(nameof(Index));
                }

                await db.ExecuteAsync("UPDATE ausencias SET estado = 'Inactivo' WHERE id_ausencia = @Id", new { Id = id });
                TempData["Exito"] = "Registro anulado.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Activar(int id)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                var fecha = await db.QueryFirstOrDefaultAsync<DateTime>("SELECT fecha_ausencia FROM ausencias WHERE id_ausencia = @Id", new { Id = id });

                var procesada = await db.QuerySingleOrDefaultAsync<int>(
                    "SELECT COUNT(1) FROM nominas WHERE @Fecha BETWEEN fecha_inicio AND fecha_fin AND estado != 'Anulada'", new { Fecha = fecha });

                if (procesada > 0)
                {
                    TempData["Error"] = "No se puede reactivar. El período de nómina de esta fecha ya fue cerrado.";
                    return RedirectToAction(nameof(Index));
                }

                await db.ExecuteAsync("UPDATE ausencias SET estado = 'Activo' WHERE id_ausencia = @Id", new { Id = id });
                TempData["Exito"] = "Registro reactivado.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
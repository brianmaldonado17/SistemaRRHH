using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Dapper;
using SistemaRRHH.Datos;
using SistemaRRHH.Models;

namespace SistemaRRHH.Controllers
{
    [Authorize(Roles = "Administrador,RecursosHumanos")]
    public class PrestacionesController : Controller
    {
        private readonly ConexionDb _conexion;

        public PrestacionesController(ConexionDb conexion)
        {
            _conexion = conexion;
        }

        // Listado de pagos realizados
        public async Task<IActionResult> Index()
        {
            using (var db = _conexion.ObtenerConexion())
            {
                var sql = @"
                    SELECT 
                        h.id_prestacion AS IdPrestacion,
                        h.id_empleado AS IdEmpleado,
                        h.tipo_prestacion AS TipoPrestacion,
                        h.fecha_calculo AS FechaCalculo,
                        h.monto_pagado AS MontoPagado,
                        h.periodo_cubierto AS PeriodoCubierto,
                        h.estado AS Estado, -- AGREGAMOS ESTADO
                        CONCAT(e.nombre, ' ', e.apellido) AS NombreEmpleado 
                    FROM historial_prestaciones h
                    INNER JOIN empleados e ON h.id_empleado = e.id_empleado
                    ORDER BY h.fecha_calculo DESC";

                var lista = await db.QueryAsync<Prestacion>(sql);
                return View(lista);
            }
        }

        // Vista para realizar un nuevo cálculo
        public async Task<IActionResult> Calcular()
        {
            using (var db = _conexion.ObtenerConexion())
            {
                ViewBag.Empleados = await db.QueryAsync("SELECT id_empleado AS IdEmpleado, CONCAT(nombre, ' ', apellido) AS NombreCompleto FROM empleados WHERE estado = 'Activo'");
            }
            return View();
        }

        // Acción para obtener el cálculo desde la DB
        [HttpPost]
        public async Task<IActionResult> ProcesarCalculo(int idEmpleado, string tipoPrestacion, int anioAplicacion)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                var empleadoInfo = await db.QueryFirstOrDefaultAsync<dynamic>(
                    "SELECT fecha_ingreso AS FechaIngreso, IFNULL(fecha_base_indemnizacion, fecha_ingreso) AS FechaBaseIndemnizacion FROM empleados WHERE id_empleado = @Id", new { Id = idEmpleado });

                DateTime fechaIngreso = empleadoInfo.FechaIngreso;
                DateTime fechaBaseIndem = empleadoInfo.FechaBaseIndemnizacion;

                if (fechaIngreso.Year > anioAplicacion)
                {
                    ViewBag.Error = $"El colaborador fue contratado en el año {fechaIngreso.Year}. No es legal pagarle prestaciones correspondientes al año {anioAplicacion}.";
                    ViewBag.Empleados = await db.QueryAsync("SELECT id_empleado AS IdEmpleado, CONCAT(nombre, ' ', apellido) AS NombreCompleto FROM empleados WHERE estado = 'Activo'");
                    return View("Calcular");
                }

                string periodoConstruido = "";

                if (tipoPrestacion == "Bono 14")
                {
                    periodoConstruido = $"Julio {anioAplicacion - 1} - Junio {anioAplicacion}";
                }
                else if (tipoPrestacion == "Aguinaldo")
                {
                    periodoConstruido = $"Diciembre {anioAplicacion - 1} - Noviembre {anioAplicacion}";
                }
                else if (tipoPrestacion == "Vacaciones")
                {
                    DateTime inicioVacaciones = new DateTime(anioAplicacion - 1, fechaIngreso.Month, fechaIngreso.Day);
                    DateTime finVacaciones = inicioVacaciones.AddYears(1).AddDays(-1);
                    periodoConstruido = $"{inicioVacaciones.ToString("dd/MM/yyyy")} al {finVacaciones.ToString("dd/MM/yyyy")}";
                }
                else if (tipoPrestacion == "Indemnizacion")
                {
                    periodoConstruido = $"{fechaBaseIndem.ToString("dd/MM/yyyy")} al {DateTime.Now.ToString("dd/MM/yyyy")}";
                }

                var duplicado = await db.QueryFirstOrDefaultAsync<int>(
                    "SELECT COUNT(1) FROM historial_prestaciones WHERE id_empleado = @Id AND tipo_prestacion = @Tipo AND periodo_cubierto = @Periodo AND estado != 'Anulada'",
                    new { Id = idEmpleado, Tipo = tipoPrestacion, Periodo = periodoConstruido });

                if (duplicado > 0)
                {
                    ViewBag.Error = $"El sistema detecta que este colaborador ya tiene registrado un pago de {tipoPrestacion} para el {periodoConstruido}.";
                    ViewBag.Empleados = await db.QueryAsync("SELECT id_empleado AS IdEmpleado, CONCAT(nombre, ' ', apellido) AS NombreCompleto FROM empleados WHERE estado = 'Activo'");
                    return View("Calcular");
                }

                var monto = await db.ExecuteScalarAsync<decimal>(
                    "SELECT fn_CalcularPrestaciones(@Id, @Tipo)",
                    new { Id = idEmpleado, Tipo = tipoPrestacion }
                );

                // GUARDAMOS COMO "GENERADA"
                var sqlInsert = @"
                    INSERT INTO historial_prestaciones (id_empleado, tipo_prestacion, fecha_calculo, monto_pagado, periodo_cubierto, estado)
                    VALUES (@Id, @Tipo, CURDATE(), @Monto, @Periodo, 'Generada')";

                await db.ExecuteAsync(sqlInsert, new { Id = idEmpleado, Tipo = tipoPrestacion, Monto = monto, Periodo = periodoConstruido });

                TempData["Exito"] = $"Cálculo de {tipoPrestacion} generado. Revise y proceda al pago.";
                return RedirectToAction(nameof(Index));
            }
        }

        // NUEVO MÉTODO: Pagar
        [HttpPost]
        public async Task<IActionResult> Pagar(int id)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                // Obtenemos qué prestación se está pagando
                var prestacion = await db.QueryFirstOrDefaultAsync<dynamic>("SELECT id_empleado AS IdEmpleado, tipo_prestacion AS TipoPrestacion FROM historial_prestaciones WHERE id_prestacion = @Id", new { Id = id });

                await db.ExecuteAsync("UPDATE historial_prestaciones SET estado = 'Pagada' WHERE id_prestacion = @Id", new { Id = id });

                // SI ES INDEMNIZACIÓN, AHORA SÍ REINICIAMOS EL CRONÓMETRO AL EMPLEADO
                if (prestacion.TipoPrestacion == "Indemnizacion")
                {
                    await db.ExecuteAsync("UPDATE empleados SET fecha_base_indemnizacion = CURDATE() WHERE id_empleado = @Id", new { Id = prestacion.IdEmpleado });
                }

                TempData["Exito"] = "La prestación ha sido marcada como Pagada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // NUEVO MÉTODO: Anular
        [HttpPost]
        public async Task<IActionResult> Anular(int id)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                var estadoActual = await db.QueryFirstOrDefaultAsync<string>("SELECT estado FROM historial_prestaciones WHERE id_prestacion = @Id", new { Id = id });

                if (estadoActual == "Pagada")
                {
                    TempData["Error"] = "Operación rechazada: No se puede anular una prestación que ya fue pagada.";
                    return RedirectToAction(nameof(Index));
                }

                await db.ExecuteAsync("UPDATE historial_prestaciones SET estado = 'Anulada' WHERE id_prestacion = @Id", new { Id = id });
                TempData["Exito"] = "El cálculo de la prestación ha sido anulado.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
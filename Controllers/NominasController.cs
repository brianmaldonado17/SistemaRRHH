using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Dapper;
using SistemaRRHH.Datos;
using SistemaRRHH.Models;
using System.Data;

namespace SistemaRRHH.Controllers
{
    [Authorize(Roles = "Administrador,RecursosHumanos")]
    public class NominasController : Controller
    {
        private readonly ConexionDb _conexion;

        public NominasController(ConexionDb conexion)
        {
            _conexion = conexion;
        }

        // Listado de todas las nóminas generadas
        public async Task<IActionResult> Index()
        {
            using (var db = _conexion.ObtenerConexion())
            {
                // Agregamos estado AS Estado
                var sql = "SELECT id_nomina AS IdNomina, tipo_nomina AS TipoNomina, fecha_inicio AS FechaInicio, fecha_fin AS FechaFin, total_pagado AS TotalPagado, estado AS Estado FROM nominas ORDER BY id_nomina DESC";
                var lista = await db.QueryAsync<Nomina>(sql);
                return View(lista);
            }
        }

        // Pantalla para configurar la nueva nómina
        public IActionResult Generar()
        {
            return View();
        }

        // Ejecutar el SP de generación
        [HttpPost]
        public async Task<IActionResult> Generar(string tipoNomina, DateTime fechaInicio, DateTime fechaFin)
        {
            int diasTotales = (fechaFin - fechaInicio).Days + 1;

            if (tipoNomina == "Quincenal" && diasTotales > 16)
            {
                ViewBag.Error = $"Error: Una nómina quincenal no puede cubrir {diasTotales} días. El máximo permitido es 16.";
                return View();
            }

            if (tipoNomina == "Mensual" && diasTotales > 31)
            {
                ViewBag.Error = $"Error: Una nómina mensual no puede cubrir {diasTotales} días. El máximo permitido es 31.";
                return View();
            }

            if (diasTotales <= 0)
            {
                ViewBag.Error = "Error: La fecha de finalización debe ser posterior a la fecha de inicio.";
                return View();
            }

            var fechaActual = DateTime.Now;
            var mesesDiferencia = ((fechaActual.Year - fechaInicio.Year) * 12) + fechaActual.Month - fechaInicio.Month;

            if (mesesDiferencia < 0 || mesesDiferencia > 1)
            {
                ViewBag.Error = "Solo se permite procesar nóminas del mes actual o del inmediato anterior.";
                return View();
            }

            using (var db = _conexion.ObtenerConexion())
            {
                var sqlValidar = "SELECT COUNT(1) FROM nominas WHERE fecha_inicio = @Inicio AND fecha_fin = @Fin AND estado != 'Anulada'";
                var existe = await db.QueryFirstOrDefaultAsync<int>(sqlValidar, new { Inicio = fechaInicio.Date, Fin = fechaFin.Date });

                if (existe > 0)
                {
                    ViewBag.Error = "Ya existe una nómina activa registrada exactamente para esas fechas.";
                    return View();
                }

                var parametros = new DynamicParameters();
                parametros.Add("p_tipo_nomina", tipoNomina);
                parametros.Add("p_fecha_inicio", fechaInicio);
                parametros.Add("p_fecha_fin", fechaFin);

                await db.ExecuteAsync("sp_GenerarNomina", parametros, commandType: CommandType.StoredProcedure);

                TempData["Exito"] = "Nómina generada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Detalles(int id)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                var sql = @"
                    SELECT 
                        d.id_detalle AS IdDetalle,
                        CONCAT(e.nombre, ' ', e.apellido) AS NombreEmpleado,
                        d.dias_trabajados AS DiasTrabajados,
                        d.bonificaciones AS Bonificaciones,
                        d.descuentos_igss AS DescuentosIgss,
                        d.otras_deducciones AS OtrasDeducciones,
                        d.total_liquido AS TotalLiquido
                    FROM detalle_nominas d
                    INNER JOIN empleados e ON d.id_empleado = e.id_empleado
                    WHERE d.id_nomina = @Id";

                var detalles = await db.QueryAsync<DetalleNomina>(sql, new { Id = id });
                ViewBag.IdNomina = id;
                return View(detalles);
            }
        }

        // NUEVO: Método para Pagar Nómina
        [HttpPost]
        public async Task<IActionResult> Pagar(int id)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                await db.ExecuteAsync("UPDATE nominas SET estado = 'Pagada' WHERE id_nomina = @Id", new { Id = id });
                TempData["Exito"] = "La nómina ha sido marcada como Pagada exitosamente. Ya no podrá ser anulada.";
                return RedirectToAction(nameof(Index));
            }
        }

        // NUEVO: Método para Anular Nómina
        [HttpPost]
        public async Task<IActionResult> Anular(int id)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                // Candado Backend: Verificamos que no intenten anular algo pagado
                var estadoActual = await db.QueryFirstOrDefaultAsync<string>("SELECT estado FROM nominas WHERE id_nomina = @Id", new { Id = id });

                if (estadoActual == "Pagada")
                {
                    TempData["Error"] = "Operación rechazada: No se puede anular una nómina que ya fue pagada.";
                    return RedirectToAction(nameof(Index));
                }

                await db.ExecuteAsync("UPDATE nominas SET estado = 'Anulada' WHERE id_nomina = @Id", new { Id = id });
                TempData["Exito"] = "La nómina ha sido anulada correctamente.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
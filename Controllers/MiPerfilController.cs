using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaRRHH.Datos;
using SistemaRRHH.Models;
using System.Security.Claims;
using System.Text;

namespace SistemaRRHH.Controllers
{
    [Authorize]
    public class MiPerfilController : Controller
    {
        private readonly ConexionDb _conexion;

        public MiPerfilController(ConexionDb conexion)
        {
            _conexion = conexion;
        }

        public async Task<IActionResult> MisPagos()
        {
            string nombreUsuarioActual = User.Identity.Name;
            using (var db = _conexion.ObtenerConexion())
            {
                var idEmpleado = await db.QueryFirstOrDefaultAsync<int>(
                    "SELECT id_empleado FROM usuarios WHERE username = @User",
                    new { User = nombreUsuarioActual });

                // AGREGAMOS ID Y ORIGEN A LA NÓMINA
                var sqlNominas = @"
                    SELECT 
                        n.id_nomina AS IdTransaccion,
                        'Nomina' AS Origen,
                        n.fecha_fin AS FechaCalculo, 
                        CONCAT(DATE_FORMAT(n.fecha_inicio, '%d/%m/%Y'), ' al ', DATE_FORMAT(n.fecha_fin, '%d/%m/%Y')) AS PeriodoCubierto, 
                        CONCAT('Nómina ', n.tipo_nomina) AS TipoPrestacion, 
                        d.total_liquido AS MontoPagado
                    FROM nominas n
                    INNER JOIN detalle_nominas d ON n.id_nomina = d.id_nomina
                    WHERE d.id_empleado = @Id AND n.estado = 'Pagada'";

                var listaNominas = await db.QueryAsync<Prestacion>(sqlNominas, new { Id = idEmpleado });

                // AGREGAMOS ID Y ORIGEN A LA PRESTACIÓN
                var sqlPrestaciones = @"
                    SELECT 
                        id_prestacion AS IdTransaccion,
                        'Prestacion' AS Origen,
                        fecha_calculo AS FechaCalculo, 
                        tipo_prestacion AS TipoPrestacion, 
                        periodo_cubierto AS PeriodoCubierto, 
                        monto_pagado AS MontoPagado
                    FROM historial_prestaciones
                    WHERE id_empleado = @Id AND estado = 'Pagada'";

                var listaPrestaciones = await db.QueryAsync<Prestacion>(sqlPrestaciones, new { Id = idEmpleado });

                var todosLosPagos = listaNominas.Concat(listaPrestaciones).OrderByDescending(x => x.FechaCalculo);
                return View(todosLosPagos);
            }
        }

        // NUEVO MÉTODO PARA GENERAR LA BOLETA
        [HttpGet]
        public async Task<IActionResult> Boleta(int id, string origen)
        {
            string nombreUsuarioActual = User.Identity.Name;
            using (var db = _conexion.ObtenerConexion())
            {
                var empleado = await db.QueryFirstOrDefaultAsync<dynamic>(@"
                    SELECT e.id_empleado, CONCAT(e.nombre, ' ', e.apellido) AS NombreCompleto, p.nombre_puesto AS Puesto 
                    FROM empleados e
                    INNER JOIN usuarios u ON e.id_empleado = u.id_empleado
                    INNER JOIN puestos p ON e.id_puesto = p.id_puesto
                    WHERE u.username = @User", new { User = nombreUsuarioActual });

                if (empleado == null) return NotFound();

                dynamic detallePago = null;

                if (origen == "Nomina")
                {
                    detallePago = await db.QueryFirstOrDefaultAsync<dynamic>(@"
                        SELECT 
                            'Sueldo Mensual' AS Concepto, n.fecha_fin AS Fecha, 
                            d.dias_trabajados AS Dias, d.bonificaciones AS Bonos, 
                            d.descuentos_igss AS IGSS, d.otras_deducciones AS ISR, d.total_liquido AS Total
                        FROM detalle_nominas d
                        INNER JOIN nominas n ON d.id_nomina = n.id_nomina
                        WHERE d.id_nomina = @Id AND d.id_empleado = @IdEmp",
                        new { Id = id, IdEmp = empleado.id_empleado });
                }
                else if (origen == "Prestacion")
                {
                    detallePago = await db.QueryFirstOrDefaultAsync<dynamic>(@"
                        SELECT 
                            tipo_prestacion AS Concepto, fecha_calculo AS Fecha, 
                            0 AS Dias, 0 AS Bonos, 0 AS IGSS, 0 AS ISR, monto_pagado AS Total
                        FROM historial_prestaciones
                        WHERE id_prestacion = @Id AND id_empleado = @IdEmp",
                        new { Id = id, IdEmp = empleado.id_empleado });
                }

                if (detallePago == null) return NotFound("Pago no encontrado o no autorizado.");

                ViewBag.Empleado = empleado;
                ViewBag.Pago = detallePago;
                return View();
            }
        }

        // 1. Mostrar la vista del formulario
        [HttpGet]
        public IActionResult CambiarPassword()
        {
            return View();
        }

        // 2. Procesar el cambio
        [HttpPost]
        public async Task<IActionResult> CambiarPassword(CambiarPassword modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            string username = User.Identity.Name;

            using (var db = _conexion.ObtenerConexion())
            {
                // Obtenemos la contraseña encriptada actual desde la base de datos
                var passwordDb = await db.QueryFirstOrDefaultAsync<string>(
                    "SELECT password_hash FROM usuarios WHERE username = @User",
                    new { User = username });


                bool passwordValida = BCrypt.Net.BCrypt.Verify(modelo.PasswordActual, passwordDb);

                if (!passwordValida)
                {
                    ModelState.AddModelError("PasswordActual", "La contraseña actual es incorrecta.");
                    return View(modelo);
                }

                // Si todo está bien, encriptamos la nueva y actualizamos
                string nuevaPasswordEncriptada = BCrypt.Net.BCrypt.HashPassword(modelo.NuevaPassword);

                await db.ExecuteAsync(
                    "UPDATE usuarios SET password_hash = @Nueva WHERE username = @User",
                    new { Nueva = nuevaPasswordEncriptada, User = username });

                TempData["Exito"] = "Tu contraseña ha sido actualizada correctamente.";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
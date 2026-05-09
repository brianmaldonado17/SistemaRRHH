using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaRRHH.Datos;
using SistemaRRHH.Models;
using System.Data;

namespace SistemaRRHH.Controllers
{
    [Authorize(Roles = "Administrador,RecursosHumanos")]
    public class EmpleadosController : Controller
    {
        private readonly ConexionDb _conexion;

        public EmpleadosController(ConexionDb conexion)
        {
            _conexion = conexion;
        }

        public async Task<IActionResult> Index()
        {
            using (var db = _conexion.ObtenerConexion())
            {
                var sql = @"
                    SELECT 
                        c.id_empleado AS IdEmpleado,
                        c.nombre AS Nombre,
                        c.apellido AS Apellido,
                        c.fecha_ingreso AS FechaIngreso,
                        c.estado AS Estado,
                        p.nombre_puesto AS NombrePuesto,
                        d.nombre_departamento AS NombreDepartamento
                    FROM empleados c
                    INNER JOIN puestos p ON c.id_puesto = p.id_puesto
                    INNER JOIN departamentos d ON c.id_departamento = d.id_departamento
                    ORDER BY c.id_empleado DESC"; // Ordenamos para ver los más recientes primero

                var lista = await db.QueryAsync<Empleado>(sql);
                return View(lista);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            using (var db = _conexion.ObtenerConexion())
            {
                ViewBag.Departamentos = await db.QueryAsync("SELECT id_departamento AS IdDepartamento, nombre_departamento AS NombreDepartamento FROM departamentos WHERE estado = 'Activo'");
                ViewBag.Puestos = await db.QueryAsync("SELECT id_puesto AS IdPuesto, nombre_puesto AS NombrePuesto FROM puestos WHERE estado = 'Activo'");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Empleado modelo)
        {
            if (ModelState.IsValid)
            {
                using (var db = _conexion.ObtenerConexion())
                {
                    var parametros = new DynamicParameters();
                    parametros.Add("p_nombre", modelo.Nombre);
                    parametros.Add("p_apellido", modelo.Apellido);
                    parametros.Add("p_fecha_ingreso", modelo.FechaIngreso);
                    parametros.Add("p_id_puesto", modelo.IdPuesto);
                    parametros.Add("p_id_departamento", modelo.IdDepartamento);

                    await db.ExecuteAsync("sp_AltaEmpleado", parametros, commandType: CommandType.StoredProcedure);

                    TempData["Exito"] = "Colaborador registrado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
            }

            using (var db = _conexion.ObtenerConexion())
            {
                ViewBag.Departamentos = await db.QueryAsync("SELECT id_departamento AS IdDepartamento, nombre_departamento AS NombreDepartamento FROM departamentos WHERE estado = 'Activo'");
                ViewBag.Puestos = await db.QueryAsync("SELECT id_puesto AS IdPuesto, nombre_puesto AS NombrePuesto FROM puestos WHERE estado = 'Activo'");
            }
            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                // CORRECCIÓN: Quitamos el SELECT * y le damos los alias exactos (AS)
                var sql = @"
                    SELECT 
                        id_empleado AS IdEmpleado, 
                        nombre AS Nombre, 
                        apellido AS Apellido, 
                        id_puesto AS IdPuesto, 
                        id_departamento AS IdDepartamento 
                    FROM empleados WHERE id_empleado = @Id";

                var empleado = await db.QueryFirstOrDefaultAsync<Empleado>(sql, new { Id = id });

                if (empleado == null) return NotFound();

                ViewBag.Departamentos = await db.QueryAsync("SELECT id_departamento AS IdDepartamento, nombre_departamento AS NombreDepartamento FROM departamentos WHERE estado = 'Activo'");
                ViewBag.Puestos = await db.QueryAsync("SELECT id_puesto AS IdPuesto, nombre_puesto AS NombrePuesto FROM puestos WHERE estado = 'Activo'");

                return View(empleado);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Empleado modelo)
        {
            if (ModelState.IsValid)
            {
                using (var db = _conexion.ObtenerConexion())
                {
                    // Al editar no tocamos el estado ni la fecha de ingreso
                    var sql = @"UPDATE empleados SET 
                        nombre = @Nombre, 
                        apellido = @Apellido, 
                        id_puesto = @IdPuesto, 
                        id_departamento = @IdDepartamento 
                        WHERE id_empleado = @IdEmpleado";

                    await db.ExecuteAsync(sql, modelo);
                    TempData["Exito"] = "Datos del colaborador actualizados correctamente.";
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> Baja(int id)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                var empleado = await db.QueryFirstOrDefaultAsync<Empleado>("SELECT id_empleado AS IdEmpleado, nombre, apellido FROM empleados WHERE id_empleado = @Id", new { Id = id });
                if (empleado == null) return NotFound();
                return View(empleado);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Baja(int IdEmpleado, string Motivo, string Observaciones)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                var parametros = new DynamicParameters();
                parametros.Add("p_id_empleado", IdEmpleado);
                parametros.Add("p_motivo", Motivo);
                parametros.Add("p_observaciones", Observaciones);

                // 1. Ejecutamos tu SP de Baja (Despido/Renuncia)
                await db.ExecuteAsync("sp_DarDeBajaEmpleado", parametros, commandType: CommandType.StoredProcedure);

                // 2. CANDADO DE SEGURIDAD: Inactivamos automáticamente su usuario del sistema
                await db.ExecuteAsync("UPDATE usuarios SET estado = 'Inactivo' WHERE id_empleado = @Id", new { Id = IdEmpleado });

                TempData["Exito"] = "Colaborador dado de baja. Sus accesos al sistema han sido revocados automáticamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // NUEVO MÉTODO: Reactivación (Recontratación)
        [HttpPost]
        public async Task<IActionResult> Activar(int id)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                // Al reactivarlo, se cambia su estado y se actualiza su fecha de ingreso y base indemnizacion al día de hoy
                var sql = "UPDATE empleados SET estado = 'Activo', fecha_ingreso = CURDATE(), fecha_base_indemnizacion = CURDATE() WHERE id_empleado = @Id";
                await db.ExecuteAsync(sql, new { Id = id });

                TempData["Exito"] = "Colaborador recontratado. Su nueva fecha de ingreso ha sido actualizada a hoy.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detalles(int id)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                var sql = @"
                    SELECT 
                        c.id_empleado AS IdEmpleado,
                        c.nombre AS Nombre,
                        c.apellido AS Apellido,
                        c.fecha_ingreso AS FechaIngreso,
                        c.estado AS Estado,
                        p.nombre_puesto AS NombrePuesto,
                        p.salario_base AS SalarioBase,
                        d.nombre_departamento AS NombreDepartamento
                    FROM empleados c
                    INNER JOIN puestos p ON c.id_puesto = p.id_puesto
                    INNER JOIN departamentos d ON c.id_departamento = d.id_departamento
                    WHERE c.id_empleado = @Id";

                var empleado = await db.QueryFirstOrDefaultAsync(sql, new { Id = id });
                if (empleado == null) return NotFound();

                return View(empleado);
            }
        }

        [HttpGet]
        public IActionResult RegistrarDesempeno(int id, string nombre)
        {
            ViewBag.IdEmpleado = id;
            ViewBag.NombreCompleto = nombre;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarDesempeno(int IdEmpleado, string MesAnio, decimal Puntualidad, decimal CalidadTrabajo, int MetasCumplidas)
        {
            decimal porcentajeMetas = (MetasCumplidas >= 10) ? 100 : (MetasCumplidas * 10);
            decimal notaFinal = (Puntualidad * 0.30m) + (CalidadTrabajo * 0.40m) + (porcentajeMetas * 0.30m);

            using (var db = _conexion.ObtenerConexion())
            {
                var sql = @"INSERT INTO indicadores_productividad 
                    (id_empleado, mes_anio, puntualidad, calidad_trabajo, metas_cumplidas, puntuacion_desempeno) 
                    VALUES (@Id, @Mes, @Puntualidad, @Calidad, @Metas, @Final)";

                await db.ExecuteAsync(sql, new
                {
                    Id = IdEmpleado,
                    Mes = MesAnio,
                    Puntualidad = Puntualidad,
                    Calidad = CalidadTrabajo,
                    Metas = MetasCumplidas,
                    Final = notaFinal
                });
            }
            TempData["Exito"] = "Evaluación de desempeño registrada.";
            return RedirectToAction(nameof(HistorialDesempeno), new { id = IdEmpleado }); // Regresamos a Detalles
        }

        [HttpGet]
        public async Task<IActionResult> HistorialDesempeno(int id)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                var empleado = await db.QueryFirstOrDefaultAsync("SELECT nombre, apellido FROM empleados WHERE id_empleado = @Id", new { Id = id });
                if (empleado == null) return NotFound();

                ViewBag.NombreCompleto = $"{empleado.nombre} {empleado.apellido}";
                ViewBag.IdEmpleado = id;

                var sql = @"
                    SELECT 
                        mes_anio AS MesAnio,
                        puntualidad AS Puntualidad,
                        calidad_trabajo AS CalidadTrabajo,
                        metas_cumplidas AS MetasCumplidas,
                        puntuacion_desempeno AS PuntuacionDesempeno
                    FROM indicadores_productividad
                    WHERE id_empleado = @Id
                    ORDER BY id_indicador DESC";

                var historial = await db.QueryAsync(sql, new { Id = id });
                return View(historial);
            }
        }
    }
}
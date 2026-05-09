using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Dapper;
using SistemaRRHH.Datos;
using SistemaRRHH.Models;

namespace SistemaRRHH.Controllers
{
    [Authorize(Roles = "Administrador")] // Solo el admin gestiona usuarios
    public class UsuariosController : Controller
    {
        private readonly ConexionDb _conexion;

        public UsuariosController(ConexionDb conexion)
        {
            _conexion = conexion;
        }

        // 1. Listar los Usuarios
        public async Task<IActionResult> Index()
        {
            using (var db = _conexion.ObtenerConexion())
            {
                var sql = @"
                    SELECT 
                        u.id_usuario AS IdUsuario, 
                        u.username AS Username, 
                        u.correo_electronico AS CorreoElectronico,
                        u.estado AS Estado,
                        CONCAT(c.nombre, ' ', c.apellido) AS NombreEmpleado, 
                        r.nombre_rol AS NombreRol
                    FROM usuarios u
                    INNER JOIN empleados c ON u.id_empleado = c.id_empleado
                    INNER JOIN roles r ON u.id_rol = r.id_rol
                    ORDER BY u.id_usuario DESC";

                var lista = await db.QueryAsync<Usuario>(sql);
                return View(lista);
            }
        }

        // 2. Mostrar la pantalla de creación
        public async Task<IActionResult> Crear()
        {
            using (var db = _conexion.ObtenerConexion())
            {
                ViewBag.Roles = await db.QueryAsync("SELECT id_rol AS IdRol, nombre_rol AS NombreRol FROM roles");

                var sqlEmpleados = @"
                    SELECT id_empleado AS IdEmpleado, CONCAT(nombre, ' ', apellido) AS NombreCompleto 
                    FROM empleados 
                    WHERE estado = 'Activo' 
                    AND id_empleado NOT IN (SELECT id_empleado FROM usuarios)";
                ViewBag.Empleados = await db.QueryAsync(sqlEmpleados);
            }

            return View();
        }

        // 3. Recibir los datos y guardarlos
        [HttpPost]
        public async Task<IActionResult> Crear(Usuario modelo)
        {
            if (ModelState.IsValid)
            {
                using (var db = _conexion.ObtenerConexion())
                {
                    string passwordHasheada = BCrypt.Net.BCrypt.HashPassword(modelo.Password);

                    // Agregamos correo_electronico al Insert si es que tu BD lo soporta desde el inicio
                    var sql = @"INSERT INTO usuarios (username, password_hash, id_empleado, id_rol, correo_electronico) 
                        VALUES (@Username, @PasswordHash, @IdEmpleado, @IdRol, @CorreoElectronico)";

                    await db.ExecuteAsync(sql, new
                    {
                        Username = modelo.Username,
                        PasswordHash = passwordHasheada,
                        IdEmpleado = modelo.IdEmpleado,
                        IdRol = modelo.IdRol,
                        CorreoElectronico = modelo.CorreoElectronico
                    });

                    TempData["Exito"] = "Usuario creado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
            }

            using (var db = _conexion.ObtenerConexion())
            {
                ViewBag.Roles = await db.QueryAsync("SELECT id_rol AS IdRol, nombre_rol AS NombreRol FROM roles");
                ViewBag.Empleados = await db.QueryAsync("SELECT id_empleado AS IdEmpleado, CONCAT(nombre, ' ', apellido) AS NombreCompleto FROM empleados WHERE estado = 'Activo' AND id_empleado NOT IN (SELECT id_empleado FROM usuarios)");
            }

            return View(modelo);
        }

        // 4. Mostrar el formulario de edición
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                // Traemos los datos del usuario. 
                var sql = "SELECT id_usuario AS IdUsuario, username AS Username, correo_electronico AS CorreoElectronico, id_rol AS IdRol, estado AS Estado FROM usuarios WHERE id_usuario = @Id";
                var usuario = await db.QueryFirstOrDefaultAsync<dynamic>(sql, new { Id = id });

                if (usuario == null) return NotFound();

                // Cargamos los roles para el select
                ViewBag.Roles = await db.QueryAsync("SELECT id_rol AS IdRol, nombre_rol AS NombreRol FROM roles");

                return View(usuario);
            }
        }

        // 5. Guardar cambios (SOLO ROL Y CORREO)
        [HttpPost]
        public async Task<IActionResult> Editar(int IdUsuario, int IdRol, string CorreoElectronico)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                var sql = "UPDATE usuarios SET id_rol = @Rol, correo_electronico = @Correo WHERE id_usuario = @Id";
                await db.ExecuteAsync(sql, new { Rol = IdRol, Correo = CorreoElectronico, Id = IdUsuario });

                TempData["Exito"] = "Permisos de usuario actualizados correctamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // 6. Anulación Lógica (Bloquear Acceso)
        [HttpPost]
        public async Task<IActionResult> Anular(int id)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                await db.ExecuteAsync("UPDATE usuarios SET estado = 'Inactivo' WHERE id_usuario = @Id", new { Id = id });

                TempData["Exito"] = "Acceso del usuario bloqueado. Ya no podrá iniciar sesión.";
                return RedirectToAction(nameof(Index));
            }
        }

        // 7. Activar Acceso
        [HttpPost]
        public async Task<IActionResult> Activar(int id)
        {
            using (var db = _conexion.ObtenerConexion())
            {
                // 1. Buscamos el estado del empleado asociado a este usuario
                var sqlValidarEmpleado = @"
                    SELECT e.estado 
                    FROM empleados e 
                    INNER JOIN usuarios u ON e.id_empleado = u.id_empleado 
                    WHERE u.id_usuario = @Id";

                var estadoEmpleado = await db.QueryFirstOrDefaultAsync<string>(sqlValidarEmpleado, new { Id = id });

                // 2. CANDADO: Si el empleado no está activo, bloqueamos la acción
                if (estadoEmpleado != "Activo")
                {
                    TempData["Error"] = "No se puede activar el usuario porque el colaborador asociado sigue 'Inactivo' en RRHH. Debe dar de alta al empleado primero.";
                    return RedirectToAction(nameof(Index));
                }

                // 3. Si el empleado está OK, activamos el usuario
                await db.ExecuteAsync("UPDATE usuarios SET estado = 'Activo' WHERE id_usuario = @Id", new { Id = id });

                TempData["Exito"] = "Acceso del usuario reactivado correctamente.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
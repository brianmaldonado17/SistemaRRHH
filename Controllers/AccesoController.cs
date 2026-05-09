using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Dapper;
using SistemaRRHH.Datos;
using SistemaRRHH.Models;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace SistemaRRHH.Controllers
{
	public class AccesoController : Controller
	{
		private readonly ConexionDb _conexion;
		private readonly IConfiguration _config;

		public AccesoController(ConexionDb conexion, IConfiguration config)
		{
			_conexion = conexion;
			_config = config;
		}

        [HttpGet]
        public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(Usuario modelo)
		{
			using (var db = _conexion.ObtenerConexion())
			{
				var sql = @"
					SELECT u.id_usuario, u.username, u.password_hash, r.nombre_rol 
					FROM usuarios u
					INNER JOIN roles r ON u.id_rol = r.id_rol
					WHERE u.username = @User
					AND u.estado = 'Activo'";

				var usuarioBd = await db.QueryFirstOrDefaultAsync(sql, new { User = modelo.Username });

				if (usuarioBd != null)
				{
					bool passwordValida = BCrypt.Net.BCrypt.Verify(modelo.Password, usuarioBd.password_hash);

					if (passwordValida)
					{
						var claims = new List<Claim>
						{
							new Claim(ClaimTypes.Name, usuarioBd.username),
							new Claim(ClaimTypes.Role, usuarioBd.nombre_rol)
						};

						var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
						await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

						return RedirectToAction("Index", "Home");
					}
				}

				ViewBag.Error = "Usuario o contraseña incorrectos";
				return View(modelo);
			}
			

			// =========================================================
			// USUARIO QUEMADO PARA PRUEBAS SIN BASE DE DATOS
			// =========================================================
			//if (modelo.Username == "admin" && modelo.Password == "123")
			//{
			//	// Le creamos el gafete (Cookie) a la fuerza con rol de Administrador
			//	var claims = new List<Claim>
			//	{
			//		new Claim(ClaimTypes.Name, "admin (Prueba)"),
			//		new Claim(ClaimTypes.Role, "Colaborador")
			//	};

			//	var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			//	await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

			//	return RedirectToAction("Index", "Home");
			//}

			//// Si escribes otra cosa que no sea admin/123
			//ViewBag.Error = "Usuario quemado incorrecto. Usa: admin / 123";
			//return View(modelo);
		}

		public async Task<IActionResult> Salir()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Login");
		}

		[HttpGet]
		public IActionResult RecuperarPassword()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> RecuperarPassword(string correoDestino)
		{
			using (var db = _conexion.ObtenerConexion())
			{
				var usuario = await db.QueryFirstOrDefaultAsync<dynamic>(
					"SELECT id_usuario, username FROM usuarios WHERE correo_electronico = @Correo AND estado = 'Activo'",
					new { Correo = correoDestino });

				if (usuario == null)
				{
					ViewBag.Error = "No se encontró un usuario activo con ese correo.";
					return View();
				}

				string nuevaClave = "Rrhh" + DateTime.Now.Year + "!";

                string claveHasheada = BCrypt.Net.BCrypt.HashPassword(nuevaClave);

                await db.ExecuteAsync("UPDATE usuarios SET password_hash = @Pass WHERE id_usuario = @Id",
					new { Pass = claveHasheada, Id = usuario.id_usuario });

				try
				{
					// Leemos los valores directamente desde el appsettings.json
					var servidor = _config["ConfiguracionEmail:ServidorSmtp"];
					var puerto = int.Parse(_config["ConfiguracionEmail:Puerto"]);
					var correoRemitente = _config["ConfiguracionEmail:CorreoRemitente"];
					var nombreRemitente = _config["ConfiguracionEmail:NombreRemitente"];
					var passwordApp = _config["ConfiguracionEmail:PasswordApp"];

					var mail = new MailMessage();
					var SmtpServer = new SmtpClient(servidor);

					mail.From = new MailAddress(correoRemitente, nombreRemitente);
					mail.To.Add(correoDestino);
					mail.Subject = "Restablecimiento de Contraseña";
					mail.Body = $"Hola {usuario.username}, tu nueva clave temporal es: {nuevaClave}";

					SmtpServer.Port = puerto;
					SmtpServer.Credentials = new NetworkCredential(correoRemitente, passwordApp);
					SmtpServer.EnableSsl = true;

					await SmtpServer.SendMailAsync(mail);
					ViewBag.Mensaje = "Contraseña enviada con éxito.";
				}
				catch
				{
					ViewBag.Error = "Error al conectar con el servidor de correo o credenciales inválidas.";
				}

				return View();
			}
		}
	}
}

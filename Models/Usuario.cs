using System.ComponentModel.DataAnnotations;

namespace SistemaRRHH.Models
{
	public class Usuario
	{
		public int IdUsuario { get; set; }

		[Required(ErrorMessage = "El nombre de usuario es obligatorio")]
		[Display(Name = "Nombre de Usuario (Username)")]
		public string Username { get; set; } = string.Empty;

		[Required(ErrorMessage = "El correo electrónico es obligatorio")]
		[EmailAddress(ErrorMessage = "Formato de correo inválido")]
		[Display(Name = "Correo Electrónico")]
		public string CorreoElectronico { get; set; } = string.Empty;

		[Required(ErrorMessage = "La contraseña es obligatoria")]
		[DataType(DataType.Password)]
		public string Password { get; set; } = string.Empty;

		[Required(ErrorMessage = "Debes seleccionar a qué empleado pertenece")]
		[Display(Name = "Empleado")]
		public int IdEmpleado { get; set; }

		[Required(ErrorMessage = "Debes asignarle un rol")]
		[Display(Name = "Rol del Sistema")]
		public int IdRol { get; set; }

		public string Estado { get; set; } = "Activo";

		// ----- Estos campos son solo para mostrar en la tabla (no se guardan en BD) -----
		public string? NombreEmpleado { get; set; }
		public string? NombreRol { get; set; }
	}
}

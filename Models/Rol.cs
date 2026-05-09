using System.ComponentModel.DataAnnotations;

namespace SistemaRRHH.Models
{
	public class Rol
	{
		public int IdRol { get; set; }

		[Required(ErrorMessage = "Nombre del rol es necesario")]
		[Display(Name = "Nombre del Rol")]
		public string NombreRol { get; set; } = string.Empty;

		public string Estado { get; set; } = "Activo";
	}
}

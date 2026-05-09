using System.ComponentModel.DataAnnotations;

namespace SistemaRRHH.Models
{
	// Esta clase es el reflejo de nuestra tabla departamentos en MySQL
	public class Departamento
	{
		public int IdDepartamento { get; set; }

		[Required(ErrorMessage = "Debe ponerle un nombre al departamento")]
		public string NombreDepartamento { get; set; } = string.Empty;

		public string Estado { get; set; } = "Activo";
	}
}

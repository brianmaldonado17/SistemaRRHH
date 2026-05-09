using System.ComponentModel.DataAnnotations;

namespace SistemaRRHH.Models
{
	// Esta clase es el reflejo de nuestra tabla departamentos en MySQL
	public class Puesto
	{
		public int IdPuesto { get; set; }

		[Required(ErrorMessage = "Debe ponerle un nombre al puesto")]
		public string NombrePuesto { get; set; } = string.Empty;

		[Required(ErrorMessage = "Debe ponerle un monto al salario base")]
		public decimal SalarioBase { get; set; }

		public string Estado { get; set; } = "Activo";
	}
}

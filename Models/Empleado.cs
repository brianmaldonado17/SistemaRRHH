using System.ComponentModel.DataAnnotations;

namespace SistemaRRHH.Models
{
	// Esta clase representa a un empleado en nuestro sistema
	public class Empleado
	{
		public int IdEmpleado { get; set; }

		[Required(ErrorMessage = "El nombre es obligatorio")]
		public string Nombre { get; set; } = string.Empty;

		[Required(ErrorMessage = "El apellido es obligatorio")]
		public string Apellido { get; set; } = string.Empty;

		[Display(Name = "Fecha de Ingreso")]
		[DataType(DataType.Date)]
		public DateTime FechaIngreso { get; set; }

		public int IdPuesto { get; set; }
		public int IdDepartamento { get; set; }
		public string Estado { get; set; } = "Activo";

		// Estas propiedades son opcionales, útiles para mostrar nombres en lugar de IDs en la tabla
		public string? NombrePuesto { get; set; }
		public string? NombreDepartamento { get; set; }
	}
}
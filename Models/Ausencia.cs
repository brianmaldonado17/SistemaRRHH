namespace SistemaRRHH.Models
{
    public class Ausencia
    {
        public int IdAusencia { get; set; }
        public int IdEmpleado { get; set; }
        public string? NombreEmpleado { get; set; } // Solo para mostrar en la lista
        public DateTime FechaAusencia { get; set; } = DateTime.Now;
        public string Motivo { get; set; } = string.Empty;
        public bool DescuentaSalario { get; set; } = true; // Por defecto asumimos que sí descuenta

        public string Estado { get; set; } = "Activo";

        public bool YaProcesada { get; set; } // Nos dirá si ya está en una nómina
    }
}
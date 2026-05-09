namespace SistemaRRHH.Models
{
    public class Prestacion
    {
        public int IdPrestacion { get; set; }
        public int IdEmpleado { get; set; }
        public string? NombreEmpleado { get; set; }
        public string TipoPrestacion { get; set; } = string.Empty;
        public DateTime FechaCalculo { get; set; } = DateTime.Now;
        public decimal MontoPagado { get; set; }
        public string? PeriodoCubierto { get; set; }
        public string Estado { get; set; } = "Generada";
        // NUEVOS CAMPOS PARA EL PDF
        public int IdTransaccion { get; set; }
        public string Origen { get; set; } = string.Empty;
    }
}
namespace SistemaRRHH.Models
{
    public class Nomina
    {
        public int IdNomina { get; set; }
        public string TipoNomina { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal TotalPagado { get; set; }
        public string Estado { get; set; } = "Generada";
    }
}

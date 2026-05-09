namespace SistemaRRHH.Models
{
    public class DetalleNomina
    {
        public int IdDetalle { get; set; }
        public int IdNomina { get; set; }
        public string? NombreEmpleado { get; set; }
        public int DiasTrabajados { get; set; }
        public decimal Bonificaciones { get; set; }
        public decimal DescuentosIgss { get; set; }
        public decimal OtrasDeducciones { get; set; } // Aquí caerá el ISR
        public decimal TotalLiquido { get; set; }
    }
}

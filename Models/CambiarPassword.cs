namespace SistemaRRHH.Models
{
    using System.ComponentModel.DataAnnotations;
    public class CambiarPassword
    {
        [Required(ErrorMessage = "Debes ingresar tu contraseña actual")]
        [DataType(DataType.Password)]
        public string PasswordActual { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nueva contraseña es obligatoria")]
        [StringLength(100, ErrorMessage = "La contraseña debe tener al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NuevaPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debes confirmar tu nueva contraseña")]
        [DataType(DataType.Password)]
        [Compare("NuevaPassword", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarPassword { get; set; } = string.Empty;
    }
}

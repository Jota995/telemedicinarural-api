

namespace Medicina.DTOs
{
    public class InsertPacienteDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? FechaNacimiento { get; set; }
        public string? Genero { get; set; } = string.Empty;
        public string? EstadoCivil { get; set; } = string.Empty;
        public string? Nacionalidad { get; set; } = string.Empty;
    }
}

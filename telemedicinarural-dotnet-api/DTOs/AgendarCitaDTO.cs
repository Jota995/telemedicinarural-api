namespace Medicina.DTOs
{
    public class AgendarCitaDTO
    {
        public string IdPaciente { get; set; }
        public string IdDoctor { get; set; }
        public string Especialidad { get; set; }
        public string Motivo { get; set; }
        public DateTime Fecha { get; set; }
    }
}

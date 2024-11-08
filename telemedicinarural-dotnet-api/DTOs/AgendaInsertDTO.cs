namespace Medicina.DTOs
{
    public class AgendaInsertDTO
    {
        public DateTime Fecha { get; set; }
        public string Estado { get; set; }
        public string Especialidad { get; set; }
        public string IdDoctor { get; set; }
    }
}

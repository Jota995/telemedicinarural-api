using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;


namespace Medicina.Models
{

    public class Cita
    {
        public ObjectId Id { get; set; }
        public ObjectId IdPaciente { get; set; } 
        public ObjectId IdDoctor { get; set; }
        public string Especialidad { get; set; }
        public string Motivo { get; set; }
        public string Estado { get; set; }
        public DateTime? Inicio { get; set; }
        public DateTime? Fin { get; set; }
        public DateTime FechaCita { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        [BsonIgnore]
        public Doctor doctor { get; set; }
        [BsonIgnore]
        public Paciente paciente { get; set; }
    }

    public class RxCita
    {
        public string Id { get; set; }
        public string IdPaciente { get; set; }
        public string idDoctor { get; set; }
        public string Especialidad { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; }
        public string Motivo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Deleted { get; set; }
    }
}

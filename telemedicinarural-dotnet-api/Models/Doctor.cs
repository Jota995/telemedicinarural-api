using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Medicina.Models
{
    public class Doctor
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("nombre")]
        public string Nombre { get; set; } = string.Empty;
        [BsonElement("fechaNacimiento")]
        public DateTime FechaNacimiento { get; set; }
        [BsonElement("genero")]
        public string Genero {  get; set; } = string.Empty;
        [BsonElement("estadoCivil")]
        public string EstadoCivil { get; set; } = string.Empty;
        [BsonElement("nacionalidad")]
        public string Nacionalidad { get; set; } = string.Empty;
        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;
        [BsonElement("especialidades")]
        public List<string> Especialidades { get; set; } 
        [BsonElement("createdAt")]
        public DateTime CreatedAt {  get; set; }
        [BsonElement("updateAt")]
        public DateTime UpdatedAt { get; set; }
        [BsonElement("idsAgenda")]
        public List<ObjectId> IdsAgenda { get; set; }
        [BsonIgnore]
        public List<AgendaMedica> agendaMedica { get; set; }
    }

    public class RxDoctor
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Genero { get; set; }
        public string EstadoCivil { get; set; }
        public string Nacionalidad { get; set; }
        public List<string> Especialidades { get; set; }
        public List<string> IdsAgenda { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

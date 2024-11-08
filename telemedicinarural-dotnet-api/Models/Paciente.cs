using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Medicina.Models
{
    public class Paciente
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("nombre")]
        public string Nombre { get; set; } = string.Empty;
        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;
        [BsonElement("fechaNacimiento")]
        public DateTime FechaNacimiento { get; set; }
        [BsonElement("genero")]
        public string Genero { get; set; } = string.Empty;
        [BsonElement("estadoCivil")]
        public string EstadoCivil { get; set; } = string.Empty;
        [BsonElement("nacionalidad")]
        public string Nacionalidad { get; set; } = string.Empty;
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }
        [BsonElement("updatedAt")]
        public DateTime UpdateAt { get; set; }
    }
}

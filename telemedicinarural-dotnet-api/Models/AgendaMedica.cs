using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Medicina.Models
{
    public class AgendaMedica
    {
        public ObjectId Id { get; set; }
        [BsonElement("fecha")]
        public DateTime Fecha { get; set; }
        [BsonElement("estado")]
        public string Estado { get; set; }
        [BsonElement("especialidad")]
        public string Especialidad { get; set; }
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }
        [BsonElement("updateAt")]
        public DateTime UpdatedAt { get; set; }
        [BsonElement("doctorId")]
        public ObjectId IdDoctor { get; set; }
        [BsonIgnore]
        public Doctor doctor { get; set; }
    }

    public class RxAgendaMedica
    {
        public string Id { get; set; }
        public string IdDoctor { get; set; }
        public string Especialidad { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

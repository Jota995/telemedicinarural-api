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
}

using Medicina.Models;
using Medicina.Services;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Numerics;

namespace Medicina.Repository
{
    public class PacienteRepository
    {
        private readonly string CollectionName = "pacientes";
        private readonly IMongoCollection<Paciente> pacienteCollection;

        public PacienteRepository(MongoDbContext context)
        {
            this.pacienteCollection = context.database.GetCollection<Paciente>(CollectionName);
        }

        public async Task<Paciente> ObtenerPaciente(string IdPaciente)
        {
            var objectIdPaciente = ObjectId.Parse(IdPaciente);
            var filter = Builders<Paciente>.Filter.Eq(d => d.Id, objectIdPaciente);
            var result = await pacienteCollection.Find(filter).FirstOrDefaultAsync();

            return result;
        }

        public async Task InsertarPaciente(Paciente paciente)
        {
            await pacienteCollection.InsertOneAsync(paciente);
        }
    }
}

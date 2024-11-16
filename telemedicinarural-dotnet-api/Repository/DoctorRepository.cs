using Medicina.Models;
using Medicina.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Numerics;

namespace Medicina.Repository
{
    public class DoctorRepository
    {
        private readonly string CollectionName = "doctores";
        private readonly IMongoCollection<Doctor> doctores;
        private readonly AgendaRepository agenda;

        public DoctorRepository(MongoDbContext context, AgendaRepository agenda)
        {
            this.doctores = context.database.GetCollection<Doctor>(CollectionName);
            this.agenda = agenda;
        }

        public async Task<List<Doctor>> Get(DateTime? updatedAt = null, int? limit = 10)
        {
            var filterBuilder = Builders<Doctor>.Filter;
            var filter = filterBuilder.Empty;

            if (updatedAt.HasValue)
            {
                var updatedAtUtc = updatedAt.Value.ToUniversalTime();
                filter = filter & filterBuilder.Gte(x => x.UpdatedAt, updatedAtUtc);
            }

            var query = doctores.Find(filter);

            if (limit.HasValue)
            {
                query = query.Limit(limit.Value);
            }

            var data = await query.ToListAsync();

            return data;
        }

        public async Task<Doctor> GetOne(string IdDoctor)
        {
            var objectIdDoctor = ObjectId.Parse(IdDoctor);
            var filter = Builders<Doctor>.Filter.Eq(d => d.Id, objectIdDoctor);
            var result = await this.doctores.Find(filter).FirstOrDefaultAsync();

            if (result.IdsAgenda is null || result.IdsAgenda.Count == 0) return  result;

            result.agendaMedica = await agenda.Get(result.IdsAgenda);

            return result;
        }

        public async Task Insert(Doctor doctor)
        {
            await doctores.InsertOneAsync(doctor);
        }

        public async Task InsertAgenda(string idDoctor, string idAgenda)
        {
            var objectIdDoctor = ObjectId.Parse(idDoctor);
            var objectIdAgenda = ObjectId.Parse(idAgenda);

            var filter = Builders<Doctor>.Filter.Eq(d => d.Id, objectIdDoctor);
            var update = Builders<Doctor>.Update.Combine(
                Builders<Doctor>.Update.Push(d => d.IdsAgenda, objectIdAgenda), // Agregar a la lista
                Builders<Doctor>.Update.Set(d => d.UpdatedAt, DateTime.UtcNow)  // Actualizar "UpdatedAt"
            );

            await doctores.UpdateOneAsync(filter,update);
        }
    }
}

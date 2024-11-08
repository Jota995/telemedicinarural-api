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

        public async Task<List<Doctor>> Get()
        {
            var data = await doctores.Find(x => true).ToListAsync();

            foreach(var doctor in data)
            {
                if(doctor.IdsAgenda is null || doctor.IdsAgenda.Count == 0) continue;

                doctor.agendaMedica = await agenda.Get(doctor.IdsAgenda);
            }

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
            var update = Builders<Doctor>.Update.Push(d => d.IdsAgenda,objectIdAgenda);

            await doctores.UpdateOneAsync(filter,update);
        }
    }
}

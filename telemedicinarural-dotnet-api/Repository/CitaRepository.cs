using Medicina.Models;
using Medicina.Services;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Medicina.Repository
{
    public class CitaRepository
    {
        private readonly string CollectionName = "citas";
        private readonly IMongoCollection<Cita> citaCollection;
        private readonly DoctorRepository doctorRepository;
        private readonly AgendaRepository agendaRepository;

        public CitaRepository(MongoDbContext context, DoctorRepository doctorRepository, AgendaRepository agendaRepository)
        {
            this.doctorRepository = doctorRepository;
            this.agendaRepository = agendaRepository;
            this.citaCollection = context.database.GetCollection<Cita>(CollectionName);
        }

        public async Task<List<Cita>> Get(DateTime? updatedAt = null, int? limit = 10, string? IdCita = null)
        {
            // Construimos el filtro dinámicamente
            var filterBuilder = Builders<Cita>.Filter;
            var filter = filterBuilder.Empty; // Esto equivale a "x => true"

            // Aplicar el filtro para `updatedAt` si se proporciona
            if (updatedAt.HasValue)
            {
                var updatedAtUtc = updatedAt.Value.ToUniversalTime();
                filter = filter & filterBuilder.Gte(x => x.UpdatedAt, updatedAtUtc);
            }

            if (!string.IsNullOrEmpty(IdCita))
            {
                var id = ObjectId.Parse(IdCita);
                filter = filter & filterBuilder.Eq(x => x.Id, id);
            }

            var query = citaCollection.Find(filter);

            if (limit.HasValue)
            {
                query = query.Limit(limit.Value);
            }

            // Ejecutar la consulta y devolver los resultados
            var results = await query.ToListAsync();

            return results;
        }

        public async Task<Cita> agendarCita(Cita cita)
        {
            var doctor = await this.doctorRepository.GetOne(cita.IdDoctor.ToString());

            if (doctor == null) throw new Exception("No existe doctor");

            var agenda = doctor.agendaMedica.Find(x => x.Fecha == cita.FechaCita);

            if (agenda == null) throw new Exception("No existe disponibilidad del doctor o agenda");

            cita.Estado = "programada";

            citaCollection.InsertOne(cita);

            agenda.Estado = "agendada";

            await agendaRepository.Update(agenda);

            return cita;
        }

        public async Task Update(Cita cita)
        {
            if (cita == null || cita.Id == ObjectId.Empty)
            {
                throw new ArgumentException("Cita inválida o ID vacío.");
            }

            var filter = Builders<Cita>.Filter.Eq(d => d.Id, cita.Id);

            var update = Builders<Cita>.Update.Combine(
                    Builders<Cita>.Update.Set(a => a.Especialidad, cita.Especialidad),
                    Builders<Cita>.Update.Set(a => a.Motivo, cita.Motivo),
                    Builders<Cita>.Update.Set(a => a.Estado, cita.Estado),
                    Builders<Cita>.Update.Set(a => a.Inicio, cita.Inicio),
                    Builders<Cita>.Update.Set(a => a.Fin, cita.Fin),
                    Builders<Cita>.Update.Set(a => a.FechaCita, cita.FechaCita),
                    Builders<Cita>.Update.Set(a => a.UpdatedAt, DateTime.UtcNow)
                );

            await citaCollection.UpdateOneAsync(filter, update);
        }
    }
}

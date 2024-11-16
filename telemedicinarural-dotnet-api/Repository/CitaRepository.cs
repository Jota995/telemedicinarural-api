using Medicina.Models;
using Medicina.Services;
using MongoDB.Driver;

namespace Medicina.Repository
{
    public class CitaRepository
    {
        private readonly string CollectionName = "citas";
        private readonly IMongoCollection<Cita> citaCollection;
        private readonly DoctorRepository doctorRepository;
        private readonly AgendaRepository agendaRepository;

        public CitaRepository(MongoDbContext context,DoctorRepository doctorRepository,AgendaRepository agendaRepository)
        {
            this.doctorRepository = doctorRepository;
            this.agendaRepository = agendaRepository;
            this.citaCollection = context.database.GetCollection<Cita>(CollectionName);
        }

        public async Task<List<Cita>> Get(DateTime? updatedAt = null, int? limit = 10)
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

            cita.Estado = "Programada";

            citaCollection.InsertOne(cita);

            await agendaRepository.UpdateAgenda(agenda);

            return cita;

        } 
    }
}

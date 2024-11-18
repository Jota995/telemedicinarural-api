using Medicina.Models;
using Medicina.Services;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Medicina.Repository
{
    public class AgendaRepository
    {
        private readonly IMongoCollection<AgendaMedica> agendaCollection;
        private readonly string CollectionName = "agenda";


        public AgendaRepository(MongoDbContext context)
        {
            this.agendaCollection = context.database.GetCollection<AgendaMedica>(CollectionName);
        }

        public async Task<List<AgendaMedica>> GetAll(string? estado = null,DateTime? updatedAt = null, int? limit = 10, string idAgenda = null)
        {
            // Construimos el filtro dinámicamente
            var filterBuilder = Builders<AgendaMedica>.Filter;
            var filter = filterBuilder.Empty; // Esto equivale a "x => true"

            // Aplicar el filtro para `updatedAt` si se proporciona
            if (updatedAt.HasValue)
            {
                var updatedAtUtc = updatedAt.Value.ToUniversalTime();
                filter = filter & filterBuilder.Gte(x => x.UpdatedAt, updatedAtUtc);
            }

            if (!string.IsNullOrEmpty(estado))
            {
                filter = filter & filterBuilder.Eq(x => x.Estado, estado);
            }

            if (!string.IsNullOrEmpty(idAgenda))
            {
                var id = ObjectId.Parse(idAgenda);
                filter = filter & filterBuilder.Eq(x => x.Id, id);
            }

            // Aplicar el límite de la consulta si se especifica
            var query = agendaCollection.Find(filter);

            if (limit.HasValue)
            {
                query = query.Limit(limit.Value);
            }

            // Ejecutar la consulta y devolver los resultados
            var results = await query.ToListAsync();

            return results;
        }

        public async Task<List<AgendaMedica>> Get(List<ObjectId> idsAgenda)
        {
            var filter = Builders<AgendaMedica>.Filter.In(x => x.Id,idsAgenda);

            var results = await agendaCollection.Find(filter).ToListAsync();

            return results;
        }

        public async Task<AgendaMedica> Insert(AgendaMedica agenda)
        {
            await this.agendaCollection.InsertOneAsync(agenda);

            return agenda;
        }

        public async Task Update(AgendaMedica agenda)
        {
            if (agenda == null || agenda.Id == ObjectId.Empty)
            {
                throw new ArgumentException("Agenda inválida o ID vacío.");
            }

            var filter = Builders<AgendaMedica>.Filter.Eq(d => d.Id, agenda.Id);

            var update = Builders<AgendaMedica>.Update.Combine(
                    Builders<AgendaMedica>.Update.Set(a => a.Fecha, agenda.Fecha),
                    Builders<AgendaMedica>.Update.Set(a => a.Estado, agenda.Estado),
                    Builders<AgendaMedica>.Update.Set(a => a.Especialidad, agenda.Especialidad),
                    Builders<AgendaMedica>.Update.Set(a => a.UpdatedAt, DateTime.UtcNow)
                );

            await agendaCollection.UpdateOneAsync(filter, update);
        }
    }
}

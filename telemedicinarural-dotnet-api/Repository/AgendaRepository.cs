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

        public async Task UpdateAgenda(AgendaMedica agenda)
        {
            var filter = Builders<AgendaMedica>.Filter.Eq(d => d.Id, agenda.Id);
            var update = Builders<AgendaMedica>.Update.Set(a => a.Estado, "agendada");

            await agendaCollection.UpdateOneAsync(filter, update);
        }
    }
}

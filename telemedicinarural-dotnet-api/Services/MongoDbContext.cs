using AspNetCore.Identity.MongoDbCore.Infrastructure;
using MongoDB.Driver;

namespace Medicina.Services
{
    public class MongoDbContext
    {
        public readonly IMongoDatabase database;

        public MongoDbContext(MongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            database = client.GetDatabase(settings.DatabaseName);
        }
    }
}

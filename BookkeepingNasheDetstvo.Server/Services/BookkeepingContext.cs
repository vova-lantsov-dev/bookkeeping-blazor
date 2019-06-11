using BookkeepingNasheDetstvo.Server.Models;
using MongoDB.Driver;

namespace BookkeepingNasheDetstvo.Server.Services
{
    internal sealed class BookkeepingContext
    {
        internal readonly IMongoCollection<Teacher> Teachers;
        internal readonly IMongoCollection<Child> Children;
        internal readonly IMongoCollection<Subject> Subjects;
        internal readonly IMongoCollection<Session> Sessions;
        internal readonly IMongoCollection<Credential> Credentials;
        internal readonly IMongoCollection<Place> Places;

        public BookkeepingContext()
        {
            var mongo = new MongoClient("mongodb://localhost:27017");
            var db = mongo.GetDatabase(nameof(BookkeepingContext));
            Teachers = db.GetCollection<Teacher>(nameof(Teachers));
            Children = db.GetCollection<Child>(nameof(Children));
            Subjects = db.GetCollection<Subject>(nameof(Subjects));
            Sessions = db.GetCollection<Session>(nameof(Sessions));
            Credentials = db.GetCollection<Credential>(nameof(Credentials));
            Places = db.GetCollection<Place>(nameof(Places));
        }
    }
}

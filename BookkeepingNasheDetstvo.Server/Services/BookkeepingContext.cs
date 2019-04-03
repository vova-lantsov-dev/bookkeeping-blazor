using System.Threading;
using BookkeepingNasheDetstvo.Server.Extensions;
using BookkeepingNasheDetstvo.Server.Models;
using Microsoft.Extensions.Hosting;
using MlkPwgen;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BookkeepingNasheDetstvo.Server.Services
{
    public class BookkeepingContext
    {
        public readonly IMongoCollection<Teacher> Teachers;
        public readonly IMongoCollection<Child> Children;
        public readonly IMongoCollection<Subject> Subjects;
        public readonly IMongoCollection<Session> Sessions;
        public readonly IMongoCollection<Credential> Credentials;
        public readonly IMongoCollection<Place> Places;

        public BookkeepingContext(IHostApplicationLifetime lifetime)
        {
            var mongo = new MongoClient("mongodb://localhost:27017");
            var db = mongo.GetDatabase(nameof(BookkeepingContext));
            Teachers = db.GetCollection<Teacher>(nameof(Teachers));
            Children = db.GetCollection<Child>(nameof(Children));
            Subjects = db.GetCollection<Subject>(nameof(Subjects));
            Sessions = db.GetCollection<Session>(nameof(Sessions));
            Credentials = db.GetCollection<Credential>(nameof(Credentials));
            Places = db.GetCollection<Place>(nameof(Places));
            
            AddDefaults(lifetime.ApplicationStopping);
        }

        private void AddDefaults(CancellationToken cancellationToken)
        {
            if (Teachers.Find(FilterDefinition<Teacher>.Empty).Any(cancellationToken))
                return;

            var teacher = new Teacher
            {
                Additional = "",
                Email = "",
                FirstName = "Лариса",
                ImageUrl = "",
                LastName = "Мантулина",
                PhoneNumber = "+380983989420",
                ReadGlobalStatistics = true,
                SecondName = "Степановна",
                EditChildren = true,
                EditTeachers = true,
                EditSubjects = true,
                PerHour = 0m,
                IsOwner = true,
                PerHourGroup = 0m,
                Id = ObjectId.GenerateNewId().ToString()
            };
            Teachers.InsertOne(teacher, cancellationToken: cancellationToken);
            
            var credential = new Credential
            {
                Id = teacher.Id,
                Salt = PasswordGenerator.Generate(8)
            };
            credential.PasswordHash = PasswordExtensions.HashPassword("w5g5jCXn", credential.Salt);
            Credentials.InsertOne(credential, cancellationToken: cancellationToken);
        }
    }
}

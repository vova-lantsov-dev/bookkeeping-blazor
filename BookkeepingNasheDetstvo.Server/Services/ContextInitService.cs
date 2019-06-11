using BookkeepingNasheDetstvo.Server.Extensions;
using BookkeepingNasheDetstvo.Server.Models;
using Microsoft.Extensions.Hosting;
using MlkPwgen;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading;
using System.Threading.Tasks;

namespace BookkeepingNasheDetstvo.Server.Services
{
    internal sealed class ContextInitService : IHostedService
    {
        private readonly BookkeepingContext _context;

        public ContextInitService(BookkeepingContext context) => _context = context;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (await _context.Teachers.Find(FilterDefinition<Teacher>.Empty).AnyAsync(cancellationToken))
                return;

            var teacher = new Teacher
            {
                Additional = "",
                Email = "",
                FirstName = "Лариса",
                ImageUrl = "",
                LastName = "Мантулина",
                PhoneNumber = "+380983989420",
                ReadGlobalStatistic = true,
                SecondName = "Степановна",
                EditChildren = true,
                EditTeachers = true,
                EditSubjects = true,
                PerHour = 0m,
                IsOwner = true,
                PerHourGroup = 0m,
                Id = ObjectId.GenerateNewId().ToString()
            };
            await _context.Teachers.InsertOneAsync(teacher, cancellationToken: cancellationToken);

            var credential = new Credential
            {
                TeacherId = teacher.Id,
                Salt = PasswordGenerator.Generate(8)
            };
            credential.PasswordHash = PasswordExtensions.HashPassword("w5g5jCXn", credential.Salt);
            await _context.Credentials.InsertOneAsync(credential, cancellationToken: cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

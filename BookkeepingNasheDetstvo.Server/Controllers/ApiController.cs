using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using BookkeepingNasheDetstvo.Server.Attributes;
using BookkeepingNasheDetstvo.Server.Extensions;
using BookkeepingNasheDetstvo.Server.Models;
using BookkeepingNasheDetstvo.Server.Models.Mvc;
using BookkeepingNasheDetstvo.Server.Services;
using Microsoft.AspNetCore.Mvc;
using MlkPwgen;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BookkeepingNasheDetstvo.Server.Controllers
{
    [Route("api")]
    [Produces("application/json")]
    [SuppressMessage("ReSharper", "UseDeconstructionOnParameter")]
    internal sealed class ApiController : Controller
    {
        private readonly BookkeepingContext _context;

        public ApiController(BookkeepingContext context) => _context = context;

        [HttpPost("authorize")]
        [ValidateModel]
        public async Task<ActionResult<string>> Authorize([FromBody] AuthorizeModel model)
        {
            var teacher = await _context.Teachers.Find(t => t.PhoneNumber == model.Login).SingleOrDefaultAsync();
            if (teacher == default)
                return NotFound();

            var credential = await _context.Credentials.Find(c => c.TeacherId == teacher.Id).SingleAsync();

            if (credential.PasswordHash != PasswordExtensions.HashPassword(model.Password, credential.Salt))
                return StatusCode(403);

            string token;
            do
            {
                token = PasswordGenerator.Generate(100);
            }
            // ReSharper disable once AccessToModifiedClosure
            while (await _context.Sessions.Find(s => s.Token == token).AnyAsync());
            
            var session = new Session
            {
                OwnerId = teacher.Id,
                Token = token
            };
            await _context.Sessions.InsertOneAsync(session);
            
            return token;
        }

        [HttpGet("current")]
        [ValidateAccessToken]
        public Teacher Current(Teacher current)
        {
            return current;
        }

        [HttpGet("teachers")]
        [ValidateAccessToken]
        public Task<List<Teacher>> ListTeachers()
        {
            return _context.Teachers.Find(FilterDefinition<Teacher>.Empty).ToListAsync();
        }

        [HttpGet("teacher/{id:required}")]
        [ValidateAccessToken]
        public Task<Teacher> GetTeacher(string id)
        {
            return _context.Teachers.Find(t => t.Id == id).SingleOrDefaultAsync();
        }

        [HttpPost("teacher")]
        [ValidateAccessToken]
        [ValidateModel]
        public async Task<ActionResult<string>> PostTeacher([FromBody] Teacher teacher)
        {
            var affectedIsOwner = await _context.Teachers.Find(t => t.Id == teacher.Id)
                .Project(t => t.IsOwner).SingleOrDefaultAsync();
            if (affectedIsOwner)
                return StatusCode(403);
            
            if (teacher.Id == null)
                teacher.Id = ObjectId.GenerateNewId().ToString();
            
            await _context.Teachers.ReplaceOneAsync(t => t.Id == teacher.Id, teacher,
                new UpdateOptions {IsUpsert = true});
            
            var newSalt = PasswordGenerator.Generate(8);
            try
            {
                await _context.Credentials.InsertOneAsync(new Credential
                {
                    Salt = newSalt,
                    TeacherId = teacher.Id,
                    PasswordHash = PasswordExtensions.HashPassword("1", newSalt)
                });
            }
            catch (MongoWriteException)
            {
                // ignored
            }

            return teacher.Id;
        }

        [HttpPost("teacher/password")]
        [ValidateAccessToken]
        [ValidateModel]
        public async Task<ActionResult> PostTeacherPassword([FromBody] PostTeacherPasswordModel model, Teacher current)
        {
            var affected = await _context.Teachers.Find(t => t.Id == model.TeacherId).SingleOrDefaultAsync();
            if (affected == default)
                return NotFound();
            
            if (!current.IsOwner && model.TeacherId != current.Id)
                return StatusCode(403);

            var newSalt = PasswordGenerator.Generate(8);
            await _context.Credentials.UpdateOneAsync(c => c.TeacherId == model.TeacherId,
                Builders<Credential>.Update.Set(c => c.Salt, newSalt)
                    .Set(c => c.PasswordHash, PasswordExtensions.HashPassword(model.NewPassword, newSalt)),
                new UpdateOptions {IsUpsert = false});
            
            return Ok();
        }

        [HttpGet("children")]
        [ValidateAccessToken]
        public Task<List<Child>> ListChildren()
        {
            return _context.Children.Find(FilterDefinition<Child>.Empty).ToListAsync();
        }

        [HttpGet("child/{id:required}")]
        [ValidateAccessToken]
        public Task<Child> GetChild(string id)
        {
            return _context.Children.Find(c => c.Id == id).SingleOrDefaultAsync();
        }

        [HttpPost("child")]
        [ValidateAccessToken]
        [ValidateModel]
        public async Task<ActionResult<string>> PostChild([FromBody] Child child, Teacher current)
        {
            if (!current.EditChildren && !current.IsOwner)
                return StatusCode(403);
            
            if (child.Id == null)
                child.Id = ObjectId.GenerateNewId().ToString();
            
            await _context.Children.ReplaceOneAsync(c => c.Id == child.Id, child, new UpdateOptions {IsUpsert = true});
            
            return child.Id;
        }

        [HttpPost("child/delete/{id:required}")]
        [ValidateAccessToken]
        public async Task<ActionResult> DeleteChild(string id, Teacher current)
        {
            if (!current.EditChildren && !current.IsOwner)
                return StatusCode(403);
            
            await _context.Children.DeleteOneAsync(c => c.Id == id);
            await _context.Subjects.UpdateManyAsync(FilterDefinition<Subject>.Empty,
                Builders<Subject>.Update.PullFilter(s => s.ChildrenIds, childId => childId == id));
            
            return Ok();
        }

        [HttpPost("teacher/delete/{id:required}")]
        [ValidateAccessToken]
        public async Task<ActionResult> DeleteTeacher(string id, Teacher current)
        {
            var affected = await _context.Teachers.Find(t => t.Id == id).Project<Teacher>("{IsOwner:1, _id:0}")
                .SingleOrDefaultAsync();
            if (affected == default)
                return NotFound();

            if (!current.EditTeachers && !current.IsOwner || affected.IsOwner)
                return StatusCode(403);
            
            await _context.Teachers.DeleteOneAsync(c => c.Id == id);
            await _context.Subjects.DeleteManyAsync(s => s.OwnerId == id);
            await _context.Sessions.DeleteManyAsync(s => s.OwnerId == id);
            await _context.Credentials.DeleteOneAsync(c => c.TeacherId == id);
            
            return Ok();
        }

        [HttpGet("subjects")]
        [ValidateAccessToken]
        public Task<List<Subject>> ListSubjects([FromQuery] string date)
        {
            return _context.Subjects.Find(s => s.Date == date).ToListAsync();
        }

        [HttpGet("subjects/allTeachers")]
        [ValidateAccessToken]
        public async Task<object> ListTeachersForSubjects()
        {
            var teachers = await _context.Teachers.Find(FilterDefinition<Teacher>.Empty)
                .Project<Teacher>("{FirstName:1, LastName:1}").ToListAsync();

            return teachers.Select(t => new { t.Id, t.FirstName, t.LastName });
        }

        [HttpGet("subjects/allChildren")]
        [ValidateAccessToken]
        public async Task<object> ListChildrenForSubjects()
        {
            var children = await _context.Children.Find(FilterDefinition<Child>.Empty)
                .Project<Child>("{FirstName:1, LastName:1, ImageUrl:1}").ToListAsync();

            return children.Select(c => new { c.Id, Name = $"{c.LastName} {c.FirstName}".Trim(), c.ImageUrl });
        }

        [HttpGet("subject/selectedChildren")]
        [ValidateAccessToken]
        public async Task<object> ListSelectedChildren([FromQuery] string subjectId)
        {
            var selectedChildrenIds = await _context.Subjects.Find(s => s.Id == subjectId).Project(s => s.ChildrenIds)
                .SingleOrDefaultAsync();
            if (selectedChildrenIds == default)
            {
                return NotFound();
            }

            var children = await _context.Children.Find(Builders<Child>.Filter.In(c => c.Id, selectedChildrenIds))
                .Project<Child>("{FirstName:1, LastName:1, ImageUrl:1}").ToListAsync();
            return children.Select(c => new { c.Id, Name = $"{c.LastName} {c.FirstName}".Trim(), c.ImageUrl });
        }

        [HttpPost("subject/removeChild")]
        [ValidateAccessToken]
        [ValidateModel]
        public async Task<ActionResult> RemoveChildFromSubject([FromBody] SubjectChildActionModel model, Teacher current)
        {
            if (!current.EditSubjects && !current.IsOwner)
                return StatusCode(403);

            await _context.Subjects.UpdateOneAsync(s => s.Id == model.SubjectId,
                Builders<Subject>.Update.PullFilter(s => s.ChildrenIds, childId => childId == model.ChildId),
                new UpdateOptions {IsUpsert = false});
            
            return Ok();
        }

        [HttpPost("subject/addChild"), ValidateAccessToken, ValidateModel]
        public async Task<ActionResult<string>> AddChildToSubject([FromBody] SubjectChildActionModel actionModel, Teacher current)
        {
            if (!current.EditSubjects && !current.IsOwner)
                return StatusCode(403);
            
            await _context.Subjects.UpdateOneAsync(s => s.Id == actionModel.SubjectId,
                Builders<Subject>.Update.AddToSet(s => s.ChildrenIds, actionModel.ChildId),
                new UpdateOptions {IsUpsert = false});

            return Ok();
        }

        [HttpGet("statistic/child/{childId:required}"), ValidateAccessToken]
        public async Task<ActionResult<object>> GetChildStatistic(DateRangeModel model, string childId, Teacher current)
        {
            if (!current.ReadGlobalStatistic && !current.IsOwner)
                return StatusCode(403);
            
            var child = await _context.Children.Find(t => t.Id == childId).SingleOrDefaultAsync();
            if (child == default)
                return NotFound();

            var subjects = await _context.Subjects.Find(FilterDefinition<Subject>.Empty)
                .Project<Subject>("{Children:1,OwnerId:1,Date:1,_id:0}").ToListAsync();
            var totalHoursByTeachers = new Dictionary<string, (int Private, int Group, int Consultation)>();
            int totalHoursPrivate = 0, totalHoursGroup = 0, totalHoursConsultation = 0;
            
            foreach (var subject in subjects)
            {
                var childInSubject = subject.ChildrenIds.Find(cId => cId == child.Id);
                if (childInSubject == default)
                    continue;

                if (!DateTime.TryParse(subject.Date, out var subjectDate) ||
                    subjectDate < model.From ||
                    subjectDate > model.To) continue;
                
                if (subject.IsConsultation)
                    totalHoursConsultation++;
                else if (subject.ChildrenIds.Count == 1)
                    totalHoursPrivate++;
                else totalHoursGroup++;
                
                totalHoursByTeachers.TryGetValue(subject.OwnerId, out var value);
                if (subject.IsConsultation)
                    value.Consultation++;
                else if (subject.ChildrenIds.Count == 1)
                    value.Private++;
                else value.Group++;
                totalHoursByTeachers[subject.OwnerId] = value;
            }
            
            var teachers = await _context.Teachers.Find(FilterDefinition<Teacher>.Empty)
                .Project<Teacher>("{ImageUrl:1,LastName:1,FirstName:1}").ToListAsync();
            var statistic = new
            {
                child.PerHour, child.PerHourGroup,
                Items = totalHoursByTeachers.Select(pair =>
                {
                    var teacher = teachers.Find(t => t.Id == pair.Key);
                    return new
                    {
                        TotalHours = new
                        {
                            pair.Value.Private,
                            pair.Value.Group,
                            pair.Value.Consultation
                        },
                        Id = pair.Key,
                        Name = $"{teacher?.LastName} {teacher?.FirstName}".Trim(),
                        ImageUrl = teacher?.ImageUrl ?? string.Empty
                    };
                }),
                TotalHours = new
                {
                    Private = totalHoursPrivate,
                    Group = totalHoursGroup,
                    Consultation = totalHoursConsultation
                },
                Name = $"{child.LastName} {child.FirstName}".Trim()
            };
            return statistic;
        }
        
        [HttpGet("statistic/teacher/{teacherId:required}"), ValidateAccessToken]
        public async Task<ActionResult<object>> GetTeacherStatistic(DateRangeModel model, string teacherId, Teacher current)
        {
            if (current.Id != teacherId && !current.ReadGlobalStatistic && !current.IsOwner)
                return StatusCode(403);
            
            var teacher = current.Id == teacherId
                ? current
                : await _context.Teachers.Find(t => t.Id == teacherId).SingleOrDefaultAsync();
            if (teacher == default)
                return NotFound();

            var subjects = await _context.Subjects.Find(s => s.OwnerId == teacher.Id)
                .Project<Subject>("{Date:1,Children:1,IsConsultation:1,_id:0}").ToListAsync();
            var totalHoursByChildren = new Dictionary<string, (int Private, int Group, int Consultation)>();
            int totalHoursPrivate = 0, totalHoursGroup = 0, totalHoursConsultation = 0;
            
            foreach (var subject in subjects)
            {
                var subjectDate = DateTime.Parse(subject.Date);
                if (subjectDate < model.From || subjectDate > model.To) continue;

                if (subject.IsConsultation)
                    totalHoursConsultation++;
                else if (subject.ChildrenIds.Count == 1)
                    totalHoursPrivate++;
                else totalHoursGroup++;
                    
                foreach (var childId in subject.ChildrenIds)
                {
                    totalHoursByChildren.TryGetValue(childId, out var value);
                    if (subject.IsConsultation)
                        value.Consultation++;
                    else if (subject.ChildrenIds.Count == 1)
                        value.Private++;
                    else value.Group++;
                    totalHoursByChildren[childId] = value;
                }
            }

            var children = await _context.Children.Find(FilterDefinition<Child>.Empty)
                .Project<Child>("{ImageUrl:1,FirstName:1,LastName:1}").ToListAsync();
            return new
            {
                teacher.PerHour, teacher.PerHourGroup,
                Items = totalHoursByChildren.Select(pair =>
                {
                    var child = children.Find(c => c.Id == pair.Key);
                    return new
                    {
                        Id = pair.Key,
                        Name = $"{child?.LastName} {child?.FirstName}".Trim(),
                        ImageUrl = child?.ImageUrl ?? string.Empty,
                        TotalHours = new
                        {
                            pair.Value.Private,
                            pair.Value.Group,
                            pair.Value.Consultation
                        }
                    };
                }),
                TotalHours = new
                {
                    Private = totalHoursPrivate,
                    Group = totalHoursGroup,
                    Consultation = totalHoursConsultation
                },
                Name = $"{teacher.LastName} {teacher.FirstName}".Trim()
            };
        }
    }
}
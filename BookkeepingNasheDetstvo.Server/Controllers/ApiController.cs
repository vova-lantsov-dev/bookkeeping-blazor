using System;
using System.Collections.Generic;
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
    public class ApiController : Controller
    {
        private readonly BookkeepingContext _context;

        public ApiController(BookkeepingContext context)
        {
            _context = context;
        }

        [HttpPost("authorize")]
        [ValidateModel]
        public async Task<ActionResult<string>> Authorize([FromBody] AuthorizeModel model)
        {
            var teacher = await _context.Teachers.Find(t => t.PhoneNumber == model.Login).SingleOrDefaultAsync();
            if (teacher == default)
                return NotFound();

            var credential = await _context.Credentials.Find(c => c.Id == teacher.Id).SingleAsync();
            if (credential == default)
                return NotFound();

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
        public ActionResult<Teacher> Current(Teacher current)
        {
            return current;
        }

        [HttpGet("teachers")]
        [ValidateAccessToken]
        public async Task<ActionResult<List<Teacher>>> ListTeachers()
        {
            return await _context.Teachers.Find(FilterDefinition<Teacher>.Empty).ToListAsync();
        }

        [HttpGet("teacher/{id:required}")]
        [ValidateAccessToken]
        public async Task<ActionResult<Teacher>> GetTeacher(string id)
        {
            return await _context.Teachers.Find(t => t.Id == id).FirstOrDefaultAsync();
        }

        [HttpPost("teacher")]
        [ValidateAccessToken]
        [ValidateModel]
        public async Task<ActionResult<string>> PostTeacher([FromBody] Teacher teacher, Teacher current)
        {
            var affectedIsOwner = await _context.Teachers.Find(t => t.Id == teacher.Id)
                .Project(t => t.IsOwner).SingleOrDefaultAsync();
            if (affectedIsOwner)
                return StatusCode(403);
            
            if (teacher.Id == null)
                teacher.Id = ObjectId.GenerateNewId().ToString();
            
            await _context.Teachers.ReplaceOneAsync(t => t.Id == teacher.Id, teacher, new UpdateOptions { IsUpsert = true });
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
            
            if (!current.IsOwner && model.TeacherId != current.Id && (affected.IsOwner || !current.EditTeachers))
                return StatusCode(403);

            var newSalt = PasswordGenerator.Generate(8);
            await _context.Credentials.UpdateOneAsync(c => c.Id == model.TeacherId,
                Builders<Credential>.Update.Set(c => c.Salt, newSalt)
                    .Set(c => c.PasswordHash, PasswordExtensions.HashPassword(model.NewPassword, newSalt)),
                new UpdateOptions {IsUpsert = false});
            
            return Ok();
        }

        [HttpGet("children")]
        [ValidateAccessToken]
        public async Task<ActionResult<List<Child>>> ListChildren()
        {
            return await _context.Children.Find(FilterDefinition<Child>.Empty).ToListAsync();
        }

        [HttpGet("child/{id:required}")]
        [ValidateAccessToken]
        public async Task<ActionResult<Child>> GetChild(string id)
        {
            return await _context.Children.Find(c => c.Id == id).FirstOrDefaultAsync();
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
            await _context.Children.ReplaceOneAsync(c => c.Id == child.Id, child, new UpdateOptions { IsUpsert = true });
            return child.Id;
        }

        [HttpPost("child/delete/{id:required}")]
        [ValidateAccessToken]
        public async Task<ActionResult> DeleteChild(string id)
        {
            await _context.Children.DeleteOneAsync(c => c.Id == id);
            await _context.Subjects.UpdateManyAsync(FilterDefinition<Subject>.Empty, Builders<Subject>.Update.PullFilter(s => s.Children, c => c.Id == id));
            return Ok();
        }

        [HttpPost("teacher/delete/{id:required}")]
        [ValidateAccessToken]
        public async Task<ActionResult> DeleteTeacher(string id, Teacher current)
        {
            var affected = await _context.Teachers.Find(t => t.Id == id).Project<Teacher>("{IsOwner:1}").SingleOrDefaultAsync();
            if (affected == default)
                return NotFound();

            if (!current.EditTeachers && !current.IsOwner || affected.IsOwner)
                return StatusCode(403);
            
            await _context.Teachers.DeleteOneAsync(c => c.Id == id);
            await _context.Subjects.DeleteManyAsync(s => s.Owner.Id == id);
            await _context.Sessions.DeleteManyAsync(s => s.OwnerId == id);
            await _context.Credentials.DeleteOneAsync(c => c.Id == id);
            
            return Ok();
        }

        [HttpGet("subjectsModel")]
        [ValidateAccessToken]
        public async Task<ActionResult<object>> ListSubjects([FromQuery] string date)
        {
            var children = await _context.Children.Find(FilterDefinition<Child>.Empty).Project<Child>("{FirstName:1, LastName:1}").ToListAsync();
            var teachers = await _context.Teachers.Find(FilterDefinition<Teacher>.Empty).Project<Teacher>("{FirstName:1, LastName:1}").ToListAsync();
            var subjects = await _context.Subjects.Find(s => s.Date == date).ToListAsync();
            
            return new
            {
                children = children.Select(c => new {c.Id, Name = $"{c.LastName} {(c.FirstName.Length == 0 ? string.Empty : $"{c.FirstName[0]}.")}".TrimEnd(' ') }),
                teachers = teachers.Select(t => new {t.Id, Name = $"{t.LastName} {(t.FirstName.Length == 0 ? string.Empty : $"{t.FirstName[0]}.")}".TrimEnd(' ') }),
                subjects
            };
        }

        [HttpPost("subject/removeChild")]
        [ValidateAccessToken]
        [ValidateModel]
        public async Task<ActionResult> RemoveSubjectChild([FromBody] RemoveSubjectChildModel model, Teacher current)
        {
            if (!current.EditSubjects && !current.IsOwner)
                return StatusCode(403);
            
            var updatedChildren = await _context.Subjects.FindOneAndUpdateAsync<Subject>(
                s => s.Date == model.Date && s.Time == model.Time && s.Owner.Id == model.OwnerId,
                Builders<Subject>.Update.PullFilter(s => s.Children, c => c.Id == model.ChildId),
                new FindOneAndUpdateOptions<Subject, Subject>
                {
                    Projection = "{Children:1}",
                    ReturnDocument = ReturnDocument.After
                });
            
            if (updatedChildren.Children.Count == 0)
                await _context.Subjects.DeleteOneAsync(s => s.Id == updatedChildren.Id);
            
            return Ok();
        }

        [HttpPost("subject/addChild")]
        [ValidateAccessToken]
        [ValidateModel]
        public async Task<ActionResult> AddSubjectChild([FromBody] AddSubjectChildModel model, Teacher current)
        {
            if (!current.EditSubjects && !current.IsOwner)
                return StatusCode(403);
            
            var id = await _context.Subjects.Find(s => s.Date == model.Date && s.Time == model.Time
                && s.Owner.Id == model.Owner.Id).Project(s => s.Id).SingleOrDefaultAsync();
            
            // TODO place identifier
            // TODO consultation
            if (id == default)
                await _context.Subjects.InsertOneAsync(new Subject
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Children = new List<IdNamePair> { model.Child },
                    Date = model.Date,
                    Time = model.Time,
                    Owner = model.Owner
                });
            else await _context.Subjects.UpdateOneAsync(s => s.Id == id, Builders<Subject>.Update.Push(s => s.Children, model.Child));
            
            return Ok();
        }
        
        // TODO update subject child method

        [HttpGet("statistic/child/{childId:required}")]
        [ValidateAccessToken]
        public async Task<ActionResult<object>> GetChildStatistic(DateRangeModel model, string childId, Teacher current)
        {
            if (!current.ReadGlobalStatistic && !current.IsOwner)
                return StatusCode(403);
            
            var child = await _context.Children.Find(t => t.Id == childId).SingleOrDefaultAsync();
            if (child == default)
                return NotFound();

            var subjects = await _context.Subjects.Find(FilterDefinition<Subject>.Empty)
                .Project<Subject>("{Children:1,Owner:1,Date:1,_id:0}").ToListAsync();
            var totalHoursByTeachers = new Dictionary<string, (string name, int total)>();
            var totalHoursGlobal = 0;
            
            foreach (var subject in subjects)
            {
                var childInSubject = subject.Children.Find(c => c.Id == child.Id);
                if (childInSubject == default)
                    continue;

                if (!DateTime.TryParse(subject.Date, out var subjectDate) ||
                    subjectDate < model.From ||
                    subjectDate > model.To) continue;
                
                if (totalHoursByTeachers.TryGetValue(subject.Owner.Id, out var value))
                    totalHoursByTeachers[subject.Owner.Id] = (value.name, value.total + 1);
                else totalHoursByTeachers.Add(subject.Owner.Id, (subject.Owner.Name, 1));
                
                ++totalHoursGlobal;
            }
            
            var teachers = await _context.Teachers.Find(FilterDefinition<Teacher>.Empty)
                .Project<Teacher>("{ImageUrl:1}").ToListAsync();
            var statistic = new
            {
                child.PerHour, child.PerHourGroup,
                sourceItems = totalHoursByTeachers.Select(pair =>
                {
                    var (id, (name, totalHours)) = pair;
                    var imageUrl = teachers.Find(t => t.Id == id)?.ImageUrl ?? string.Empty;
                    return new { totalHours, id, name, imageUrl };
                }),
                totalHours = totalHoursGlobal,
                name = $"{child.LastName} {child.FirstName}".Trim()
            };
            return statistic;
        }
        
        [HttpGet("statistic/teacher/{teacherId:required}")]
        [ValidateAccessToken]
        public async Task<ActionResult<object>> GetTeacherStatistic(DateRangeModel model, string teacherId, Teacher current)
        {
            if (current.Id != teacherId && !current.ReadGlobalStatistic && !current.IsOwner)
                return StatusCode(403);
            
            var teacher = current.Id == teacherId
                ? current
                : await _context.Teachers.Find(t => t.Id == teacherId).SingleOrDefaultAsync();
            if (teacher == default)
                return NotFound();

            var subjects = await _context.Subjects.Find(s => s.Owner.Id == teacher.Id)
                .Project<Subject>("{Date:1,Children:1,_id:0}").ToListAsync();
            var totalHoursByChildren = new Dictionary<string, (string name, int total)>();
            var totalHoursGlobal = 0;
            
            foreach (var subject in subjects)
            {
                var subjectDate = DateTime.Parse(subject.Date);
                if (subjectDate < model.From || subjectDate > model.To) continue;
                    
                foreach (var child in subject.Children)
                {
                    if (totalHoursByChildren.TryGetValue(child.Id, out var value))
                        totalHoursByChildren[child.Id] = (value.name, value.total + 1);
                    else totalHoursByChildren.Add(child.Id, (child.Name, 1));
                    
                    ++totalHoursGlobal;
                }
            }

            var children = await _context.Children.Find(FilterDefinition<Child>.Empty).Project<Child>("{ImageUrl:1}").ToListAsync();
            return new
            {
                teacher.PerHour, teacher.PerHourGroup,
                sourceItems = totalHoursByChildren.Select(pair =>
                {
                    var (id, (name, totalHours)) = pair;
                    var imageUrl = children.Find(c => c.Id == id)?.ImageUrl ?? string.Empty;
                    return new { totalHours, id, name, imageUrl };
                }),
                totalHours = totalHoursGlobal,
                name = $"{teacher.LastName} {teacher.FirstName}".Trim()
            };
        }
    }
}
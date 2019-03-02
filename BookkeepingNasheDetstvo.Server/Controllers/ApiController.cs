using BookkeepingNasheDetstvo.Server.Models;
using BookkeepingNasheDetstvo.Server.Services;
using BookkeepingNasheDetstvo.Shared;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BookkeepingNasheDetstvo.Server.Extensions;

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
        public async Task<ActionResult<string>> Authorize([FromBody] LogInModel model)
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
                token = MlkPwgen.PasswordGenerator.Generate(100);
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
        public async Task<ActionResult<string>> PostTeacher([FromBody] Teacher teacher)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            
            if (teacher.Id == null)
                teacher.Id = ObjectId.GenerateNewId().ToString();
            await _context.Teachers.ReplaceOneAsync(t => t.Id == teacher.Id, teacher, new UpdateOptions { IsUpsert = true });
            return teacher.Id;
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
        public async Task<ActionResult<string>> PostChild([FromBody] Child child)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            
            if (child.Id == null)
                child.Id = ObjectId.GenerateNewId().ToString();
            await _context.Children.ReplaceOneAsync(c => c.Id == child.Id, child, new UpdateOptions { IsUpsert = true });
            return child.Id;
        }

        [HttpPost("child/delete")]
        [ValidateAccessToken]
        public async Task<ActionResult> DeleteChild([FromBody] string id)
        {
            if (id == null)
                return BadRequest();
            
            await _context.Children.DeleteOneAsync(c => c.Id == id);
            await _context.Subjects.UpdateManyAsync(FilterDefinition<Subject>.Empty, Builders<Subject>.Update.PullFilter(s => s.Children, c => c.Id == id));
            return Ok();
        }

        [HttpPost("teacher/delete")]
        [ValidateAccessToken]
        public async Task<ActionResult> DeleteTeacher([FromBody] string id)
        {
            if (id == null)
                return BadRequest();
            
            await _context.Teachers.DeleteOneAsync(c => c.Id == id);
            await _context.Subjects.DeleteManyAsync(s => s.Owner.Id == id);
            return Ok();
        }

        [HttpGet("subjectsModel")]
        [ValidateAccessToken]
        public async Task<ActionResult<Day>> ListSubjects([FromQuery] string date)
        {
            var children = await _context.Children.Find(FilterDefinition<Child>.Empty).Project<Child>("{FirstName:1, LastName:1}").ToListAsync();
            var teachers = await _context.Teachers.Find(FilterDefinition<Teacher>.Empty).Project<Teacher>("{FirstName:1, LastName:1}").ToListAsync();
            var subjects = await _context.Subjects.Find(s => s.Date == date).ToListAsync();
            return new Day
            {
                Children = children.Select(c => new IdNamePair { Id = c.Id, Name = $"{c.LastName} {(c.FirstName.Length == 0 ? string.Empty : $"{c.FirstName[0]}.")}".TrimEnd(' ') }).ToList(),
                Teachers = teachers.Select(t => new IdNamePair { Id = t.Id, Name = $"{t.LastName} {(t.FirstName.Length == 0 ? string.Empty : $"{t.FirstName[0]}.")}".TrimEnd(' ') }).ToList(),
                Subjects = subjects
            };
        }

        [HttpPost("subject/removeChild")]
        [ValidateAccessToken]
        public async Task<ActionResult> RemoveSubjectChild([FromBody] RemoveChildAction model)
        {
            Expression<Func<Subject, bool>> filterExpr = s => s.Date == model.Date && s.Time == model.Time
                && s.Owner.Id == model.OwnerId;
            var updatedChildren = await _context.Subjects.FindOneAndUpdateAsync(filterExpr,
                Builders<Subject>.Update.PullFilter(s => s.Children, c => c.Id == model.ChildId), new FindOneAndUpdateOptions<Subject, Subject>
                {
                    Projection = "{Children:1, _id:0}",
                    ReturnDocument = ReturnDocument.After
                });
            if (updatedChildren.Children.Count == 0)
                await _context.Subjects.DeleteOneAsync(filterExpr);
            return Ok();
        }

        [HttpPost("subject/addChild")]
        [ValidateAccessToken]
        public async Task<ActionResult> AddSubjectChild([FromBody] AddChildAction model)
        {
            Expression<Func<Subject, bool>> filterExpr = s => s.Date == model.Date && s.Time == model.Time
                && s.Owner.Id == model.Owner.Id;
            bool subjectExists = await _context.Subjects.Find(filterExpr).AnyAsync();
            if (!subjectExists)
                await _context.Subjects.InsertOneAsync(new Subject
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Children = new List<IdNamePair> { model.Child },
                    Date = model.Date,
                    Time = model.Time,
                    Owner = model.Owner
                });
            else await _context.Subjects.UpdateOneAsync(filterExpr, Builders<Subject>.Update.Push(s => s.Children, model.Child));
            return Ok();
        }

        [HttpGet("statistic/{type:required}/{id:required}")]
        [ValidateAccessToken]
        public async Task<ActionResult<Statistic>> GetStatisticsFor([FromQuery] string from, [FromQuery] string to, string type, string id)
        {
            if (from == null || to == null || (type != "teacher" && type != "child") || !DateTime.TryParseExact(from, "d-M-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fromValue) || !DateTime.TryParseExact(to, "d-M-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var toValue))
                return BadRequest();
            
            if (type == "teacher")
            {
                var teacher = await _context.Teachers.Find(t => t.Id == id).Project<Teacher>("{PerHour:1, FirstName:1, LastName:1}").SingleOrDefaultAsync();
                if (teacher == default)
                    return NotFound();

                var subjects = await _context.Subjects.Find(s => s.Owner.Id == teacher.Id).Project<Subject>("{Date:1,Children:1,_id:0}").ToListAsync();
                var totalHoursByChildren = new Dictionary<string, (string name, int total)>();
                var totalHours = 0;
                foreach (var subject in subjects)
                {
                    if (!DateTime.TryParse(subject.Date, out var subjectDate) || subjectDate < fromValue ||
                        subjectDate > toValue) continue;
                    
                    foreach (var child in subject.Children)
                    {
                        if (totalHoursByChildren.TryGetValue(child.Id, out var value))
                            totalHoursByChildren[child.Id] = (value.name, value.total + 1);
                        else totalHoursByChildren.Add(child.Id, (child.Name, 1));
                        ++totalHours;
                    }
                }

                var children = await _context.Children.Find(FilterDefinition<Child>.Empty).Project<Child>("{Icon:1}").ToListAsync();
                var statistic = new Statistic
                {
                    PerHour = teacher.PerHour,
                    SourceItems = totalHoursByChildren.Select(pair => new SourceItem
                    {
                        TotalHours = pair.Value.total,
                        Id = pair.Key,
                        Name = pair.Value.name,
                        ImageUrl = children.Find(c => c.Id == pair.Key)?.ImageUrl ?? string.Empty
                    }).ToList(),
                    TotalHours = totalHours,
                    Name = $"{teacher.LastName} {teacher.FirstName}".Trim()
                };
                return statistic;
            }
            else
            {
                var child = await _context.Children.Find(t => t.Id == id).Project<Child>("{PerHour:1, FirstName:1, LastName: 1}").SingleOrDefaultAsync();
                if (child == default)
                    return NotFound();

                var subjects = await _context.Subjects.Find(FilterDefinition<Subject>.Empty).Project<Subject>("{Children:1,Owner:1,Date:1,_id:0}").ToListAsync();
                var totalHoursByTeachers = new Dictionary<string, (string name, int total)>();
                var totalHours = 0;
                foreach (var subject in subjects)
                {
                    var childInSubject = subject.Children.Find(c => c.Id == child.Id);
                    if (childInSubject == default)
                        continue;

                    if (!DateTime.TryParse(subject.Date, out var subjectDate) || subjectDate < fromValue ||
                        subjectDate > toValue) continue;
                    
                    if (totalHoursByTeachers.TryGetValue(subject.Owner.Id, out var value))
                        totalHoursByTeachers[subject.Owner.Id] = (value.name, value.total + 1);
                    else totalHoursByTeachers.Add(subject.Owner.Id, (subject.Owner.Name, 1));
                    ++totalHours;
                }
                var teachers = await _context.Teachers.Find(FilterDefinition<Teacher>.Empty).Project<Teacher>("{Icon:1}").ToListAsync();
                var statistic = new Statistic
                {
                    PerHour = child.PerHour,
                    SourceItems = totalHoursByTeachers.Select(pair => new SourceItem
                    {
                        TotalHours = pair.Value.total,
                        Id = pair.Key,
                        Name = pair.Value.name,
                        ImageUrl = teachers.Find(c => c.Id == pair.Key)?.ImageUrl ?? string.Empty
                    }).ToList(),
                    TotalHours = totalHours,
                    Name = $"{child.LastName} {child.FirstName}".Trim()
                };
                return statistic;
            }
        }

        public class RemoveSubjectItemModel
        {
            public string Id;
            public string ItemId;
        }
    }
}
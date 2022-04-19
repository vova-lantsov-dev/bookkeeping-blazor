using Bookkeeping.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookkeeping.Data;

public sealed class BookkeepingContext : DbContext
{
	public const string Schema = "public";
	
	public BookkeepingContext(DbContextOptions<BookkeepingContext> opts) : base(opts)
	{
	}
	
	public DbSet<Child> Children { get; set; }
	public DbSet<Teacher> Teachers { get; set; }
	public DbSet<Subject> Subjects { get; set; }
	public DbSet<Location> Locations { get; set; }
	public DbSet<Subject__Child> SubjectsChildrenReference { get; set; }
}
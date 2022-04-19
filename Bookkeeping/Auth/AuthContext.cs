using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bookkeeping.Auth;

public sealed class AuthContext : IdentityDbContext<TeacherIdentityUser>
{
	public AuthContext(DbContextOptions<AuthContext> opts) : base(opts) 
	{
	}
}
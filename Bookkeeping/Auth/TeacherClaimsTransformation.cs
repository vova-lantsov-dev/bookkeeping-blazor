using System.Security.Claims;
using Bookkeeping.Data;
using Bookkeeping.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace Bookkeeping.Auth;

internal sealed class TeacherClaimsTransformation : IClaimsTransformation
{
	private readonly BookkeepingContext _context;

	public TeacherClaimsTransformation(BookkeepingContext context)
	{
		_context = context;
	}
	
	public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
	{
		if (principal.HasClaim(claim => claim.Type == CustomClaimTypes.TeacherId))
			return principal;
		
		string? username = principal.FindFirst(ClaimTypes.Name)?.Value;
		if (username == null)
		{
			throw new Exception("Username claim was not found");
		}
		
		Teacher? teacher = await _context.Teachers
			.AsNoTracking()
			.Include(t => t.Permissions)
			.FirstOrDefaultAsync(t => t.AuthUserName == username);
		if (teacher == null)
		{
			throw new Exception("Teacher with specified username was not found");
		}

		var teacherIdentity = new ClaimsIdentity();
		teacherIdentity.AddClaim(new Claim(CustomClaimTypes.TeacherId, teacher.Id.ToString(),
			ClaimValueTypes.Integer32));
		if (teacher.Permissions.ReadGlobalStatistic)
			teacherIdentity.AddClaim(new Claim(CustomClaimTypes.ReadGlobalStatistic, true.ToString(), ClaimValueTypes.Boolean));
		principal.AddIdentity(teacherIdentity);

		return principal;
	}
}
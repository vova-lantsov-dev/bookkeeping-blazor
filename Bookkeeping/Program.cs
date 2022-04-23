using Bookkeeping.Auth;
using Bookkeeping.Data;
using Bookkeeping.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy(CustomClaimTypes.ReadGlobalStatistic, policy => policy
        .RequireAuthenticatedUser().RequireClaim(CustomClaimTypes.ReadGlobalStatistic));
});

builder.Services
    .AddDefaultIdentity<TeacherIdentityUser>(opts =>
    {
        opts.Password.RequiredLength = 8;
        opts.Password.RequiredUniqueChars = 4;
        opts.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<AuthContext>();

builder.Services.AddTransient<IClaimsTransformation, TeacherClaimsTransformation>();

builder.Services.AddDbContext<AuthContext>(opts =>
{
#if DEBUG
    opts.UseInMemoryDatabase("auth");
#else
    opts.UseNpgsql(builder.Configuration.GetConnectionString("AuthConnectionString"), npgsql =>
    {
        npgsql.CommandTimeout(15);
    });
#endif
});

builder.Services.AddDbContextFactory<BookkeepingContext>(opts =>
{
#if DEBUG
    opts.UseInMemoryDatabase("bookkeeping");
#else
    opts.UseNpgsql(builder.Configuration.GetConnectionString("BookkeepingConnectionString"), npgsql =>
    {
        npgsql.CommandTimeout(15);
    });
#endif
});

WebApplication app = builder.Build();

await using (AsyncServiceScope scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateAsyncScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<TeacherIdentityUser>>();
    var bookkeepingContext = scope.ServiceProvider.GetRequiredService<BookkeepingContext>();

    if (await userManager.FindByNameAsync("admin") == null)
    {
        IdentityResult? result = await userManager.CreateAsync(new TeacherIdentityUser { Email = "admin@admin", UserName = "admin" },
            "Password12345");
        if (result is not { Succeeded: true })
        {
            throw new Exception();
        }
    }

    if (!await bookkeepingContext.Teachers.AnyAsync())
    {
        bookkeepingContext.Teachers.Add(new Teacher
        {
            Email = "admin@admin",
            FirstName = "admin",
            LastName = "",
            Patronymic = "",
            PerHour = 0M,
            PerHourGroup = 0M,
            Permissions = new TeacherPermissions
            {
                EditChildren = true,
                EditSubjects = true,
                EditTeachers = true,
                IsOwner = true,
                ReadGlobalStatistic = true
            },
            PhoneNumber = "",
            AuthUserName = "admin"
        });
        await bookkeepingContext.SaveChangesAsync();
    }
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

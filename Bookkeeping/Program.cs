using Bookkeeping.Auth;
using Bookkeeping.Data;
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
    })
    .AddEntityFrameworkStores<AuthContext>();

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
    var signInManager = scope.ServiceProvider.GetRequiredService<UserManager<TeacherIdentityUser>>();
    await signInManager.CreateAsync(new TeacherIdentityUser { Email = "admin@admin", UserName = "admin" },
        "My_Super_Password_12345");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

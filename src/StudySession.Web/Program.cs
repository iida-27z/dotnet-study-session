using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.CookiePolicy;
using StudySession.Application.Abstractions.Security;
using StudySession.Application.AttendanceSummary;
using StudySession.Application.Authentication;
using StudySession.Application.StudySessions;
using StudySession.Domain.Participation;
using StudySession.Domain.StudySessions;
using StudySession.Domain.Users;
using StudySession.Infrastructure.Data;
using StudySession.Infrastructure.QueryServices;
using StudySession.Infrastructure.Repositories;
using StudySession.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/Login");
    options.Conventions.AllowAnonymousToPage("/Error403");
    options.Conventions.AllowAnonymousToPage("/Error404");
    options.Conventions.AllowAnonymousToPage("/Error500");
});

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/Error403";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
    options.Secure = CookieSecurePolicy.SameAsRequest;
});

builder.Services.AddScoped<IStudySessionDbConnectionFactory, StudySessionDbConnectionFactory>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IStudySessionRepository, StudySessionRepository>();
builder.Services.AddScoped<IStudySessionParticipationRepository, StudySessionParticipationRepository>();
builder.Services.AddScoped<IStudySessionParticipationSummaryQueryService, StudySessionParticipationSummaryQueryService>();
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<StudySessionTimeZone>();
builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<CreateStudySessionUseCase>();
builder.Services.AddScoped<GetAttendanceSummaryUseCase>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseExceptionHandler("/Error500");
app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;
    var path = context.HttpContext.Request.Path;

    if (path.StartsWithSegments("/Error", StringComparison.OrdinalIgnoreCase))
    {
        return;
    }

    if (response.StatusCode == StatusCodes.Status404NotFound)
    {
        response.Redirect("/Error404");
        return;
    }

    if (response.StatusCode == StatusCodes.Status403Forbidden)
    {
        response.Redirect("/Error403");
    }
});

app.UseCookiePolicy();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets().AllowAnonymous();
app.MapRazorPages().WithStaticAssets();

app.Run();

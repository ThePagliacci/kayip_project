using kayip_project.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using kayip_project.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using kayip_project.Repository.IRepository;
using kayip_project.Repository;
using Microsoft.AspNetCore.Authentication.Google;
using kayip_project.Models;
using Microsoft.AspNetCore.Mvc;
using AspNetCore.ReCaptcha;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
    options.HttpsPort = 7003; // Ensure this matches your HTTPS port in launchSettings.json
});

//ValidateAntiForgeryToken
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

ReplacePlaceholdersWithSecrets(builder.Configuration);


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddRazorPages();
builder.Services.AddIdentity<IdentityUser, IdentityRole>(
    options => 
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Lockout.MaxFailedAccessAttempts = 7;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20);
    }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();


builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork> ();

//google auth
var services = builder.Services;
var configuration = builder.Configuration;
services.AddAuthentication().AddGoogle(GoogleOptions =>
{
    GoogleOptions.ClientId = configuration["Authentication:Google:ClientId"];
    GoogleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
});
services.AddRazorPages();
    
// Add ReCaptcha
services.AddReCaptcha(configuration.GetSection("ReCaptcha"));

//cookie time out
services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".AspNetCore.Identity.Application";
    options.ExpireTimeSpan = TimeSpan.FromHours(4);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; 
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();

void ReplacePlaceholdersWithSecrets(IConfiguration configuration)
{
    var recaptchaSection = configuration.GetSection("ReCaptcha");
    
    // Fetching from environment variables
    recaptchaSection["SiteKey"] = Environment.GetEnvironmentVariable("reCAPTCHA:SiteKey") 
        ?? recaptchaSection["SiteKey"];
    recaptchaSection["SecretKey"] = Environment.GetEnvironmentVariable("reCAPTCHA:SecretKey") 
        ?? recaptchaSection["SecretKey"];
}

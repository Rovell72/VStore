using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ILocalizer, Localizer>();
builder.Services.AddControllersWithViews(o => o.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(
    builder.Configuration.GetConnectionString("Default"),
    sqlOptions => sqlOptions.EnableRetryOnFailure(
        maxRetryCount: 5,
        maxRetryDelay: TimeSpan.FromSeconds(30),
        errorNumbersToAdd: null)));
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
var googleConfigured = !string.IsNullOrWhiteSpace(googleClientId) && !string.IsNullOrWhiteSpace(googleClientSecret);
builder.Configuration["Authentication:Google:Configured"] = googleConfigured.ToString();

var authBuilder = builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Account/Login";
        o.AccessDeniedPath = "/Account/Login";
        o.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

if (googleConfigured)
{
    authBuilder.AddGoogle(GoogleDefaults.AuthenticationScheme, o =>
    {
        o.ClientId = googleClientId!;
        o.ClientSecret = googleClientSecret!;
        o.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        o.CallbackPath = "/signin-google";
        o.Scope.Add("email");
        o.Scope.Add("profile");
    });
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Attempting to initialize database...");
        db.Database.EnsureCreated();
        DbSeeder.Seed(db);
        logger.LogInformation("Database initialized successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to initialize database. Error: {ErrorMessage}. Ensure SQL Server LocalDB is installed and running. Connection String: {ConnectionString}", 
            ex.Message, builder.Configuration.GetConnectionString("Default"));
        // Don't re-throw to allow app to start, but log the error
    }
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.Run();

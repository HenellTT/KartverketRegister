using KartverketRegister.Auth;
using KartverketRegister.Utils;
using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// ✅ Initialize DB (using your existing logic)
bool connectedToDb = false;
SequelInit? seq = null;
int attempt = 0;

while (!connectedToDb)
{
    attempt++;
    try
    {
        seq = new SequelInit(Constants.DataBaseIp, Constants.DataBaseName);
        seq.conn.Open();
        seq.InitDb();
        seq.conn.Close();
        connectedToDb = true;
        Console.WriteLine($"Connected to DB at {Constants.DataBaseIp}:{Constants.DataBasePort} (attempt {attempt}).");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Connection to DB failed at {Constants.DataBaseIp}:{Constants.DataBasePort} with password: {Constants.DataBaseRootPassword}");
        Console.WriteLine($"Error message {ex.Message}");
        Console.WriteLine("Retrying in 2s...");
        Constants.DataBaseRootPassword = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
        Thread.Sleep(2000);
    }
}

// Capture confirmed non-null connection string for DI registration
var dbConnString = seq!.dbConnString;

// ✅ Register a scoped MySQL connection factory using SequelInit's connection string
builder.Services.AddScoped<MySqlConnection>(_ =>
{
    var conn = new MySqlConnection(dbConnString);
    conn.Open();
    return conn;
});

// ✅ Identity setup (custom user/role stores)
builder.Services.AddScoped<IUserStore<AppUser>>(sp => new MySqlUserStore(dbConnString));
builder.Services.AddScoped<IRoleStore<IdentityRole<int>>, MySqlRoleStore>();

builder.Services.AddIdentity<AppUser, IdentityRole<int>>(options =>
{
    options.User.RequireUniqueEmail = true;
    if (!Constants.RequireStrongPassword)
    {
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequiredLength = 4;
    }
})
.AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    // Redirect here if the user is NOT authenticated
    options.LoginPath = "/Auth/Login";

    // Redirect here if the user IS authenticated but forbidden (403)
    options.AccessDeniedPath = "/Auth/AccessDenied";

    // Redirect here after logout
    options.LogoutPath = "/Auth/Logout";

    // Session and expiration settings
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
    options.SlidingExpiration = true;
});

builder.Services.AddAuthorization();

var app = builder.Build();




// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// ✅ Identity middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
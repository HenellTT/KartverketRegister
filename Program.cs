using KartverketRegister.Utils;
using KartverketRegister.Auth;
using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// ✅ Initialize DB (using your existing logic)
bool connectedToDb = false;
SequelInit seq = null;

while (!connectedToDb)
{
    try
    {
        seq = new SequelInit(Constants.DataBaseIp, Constants.DataBaseName);
        seq.conn.Open();
        seq.InitDb();
        seq.conn.Close();
        connectedToDb = true;
    }
    catch
    {
        Console.WriteLine($"Connection to DB failed: No database found at {Constants.DataBaseIp}:{Constants.DataBasePort}");
        Console.WriteLine("Retrying in 2s...");
        Thread.Sleep(2000);
    }
}

// ✅ Register a scoped MySQL connection factory using SequelInit's connection string
builder.Services.AddScoped<MySqlConnection>(_ =>
{
    var conn = new MySqlConnection(seq.dbConnString);
    conn.Open();
    return conn;
});

// ✅ Identity setup (custom user/role stores)
builder.Services.AddScoped<IUserStore<AppUser>, MySqlUserStore>();
builder.Services.AddScoped<IRoleStore<IdentityRole<int>>, MySqlRoleStore>();

builder.Services.AddIdentity<AppUser, IdentityRole<int>>(options =>
{
    options.User.RequireUniqueEmail = false;
})
.AddDefaultTokenProviders();

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

using KartverketRegister.Utils;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
SequelInit seq;
bool ConnectedToDb = false;
while (!ConnectedToDb)
{
    try
    {
        seq = new SequelInit(Constants.DataBaseIp, Constants.DataBaseName);
        seq.conn.Open();
        seq.InitDb();
        seq.conn.Close();
        // Funny but real, men programmet kræsjer hvis den ikke får kobla opp mot database. Så teknisk sett er det et test i seg selv
        ConnectedToDb = true;
    }
    catch (Exception ex) {
        Console.WriteLine($"Connection to db failed: No database found at {Constants.DataBaseIp}:{Constants.DataBasePort}");
        Console.WriteLine("Retrying in 2s. . .");
        Thread.Sleep(2000);
    }
}

app.Run();

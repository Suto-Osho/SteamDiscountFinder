using Microsoft.EntityFrameworkCore;
using SteamDiscountFinder.Data;
using SteamDiscountFinder.Service;
using SteamDiscountFinder.Service.Interfaces;
using SteamDiscountFinder.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddRazorPages();

builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseSqlite("Data Source=C:\\Users\\osho\\source\\repos\\SteamDiscountFinder\\SteamDiscountFinder.Data\\games.db")); // Reuse same DB
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddScoped<ISteamService, SteamService>();
builder.Services.AddScoped<CheapSharkService>();
builder.Services.AddHttpClient();


var app = builder.Build();

// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
// Comment out HTTPS stuff if needed
// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run("http://localhost:5050"); // Use a known working port
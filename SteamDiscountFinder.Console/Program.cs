using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SteamDiscountFinder.Data;
using SteamDiscountFinder.Service;
using SteamDiscountFinder.Service.Interfaces;

class Program
{
    static async Task Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddDbContext<GameDbContext>(options =>
                options.UseSqlite("Data Source=C:\\Users\\osho\\source\\repos\\SteamDiscountFinder\\SteamDiscountFinder.Data\\games.db")) // Ensure the same connection string here
            .AddSingleton<HttpClient>()
            .AddScoped<ISteamService, SteamService>()
            .BuildServiceProvider();


        using var scope = serviceProvider.CreateScope();
        var steamService = scope.ServiceProvider.GetRequiredService<ISteamService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();

        // Fetch discounted games
        var games = await steamService.GetDiscountedGamesAsync();

        Console.WriteLine("🎮 Steam Discounted Games:\n");

        foreach (var game in games)
        {
            Console.WriteLine($"Title: {game.Title}");
            Console.WriteLine($"Original Price: ${game.OriginalPrice}");
            Console.WriteLine($"Discounted Price: ${game.DiscountedPrice}");
            Console.WriteLine($"URL: {game.Url}");
            Console.WriteLine(new string('-', 40));
        }

        // Save to database
        dbContext.Games.AddRange(games);
        await dbContext.SaveChangesAsync();
        Console.WriteLine("\n Games saved to the database.");
    }
}

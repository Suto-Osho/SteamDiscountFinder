using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SteamDiscountFinder.Data
{
    /// <summary>
    /// Factory used for creating the DbContext during design-time operations.
    /// </summary>
    public class GameDbContextFactory : IDesignTimeDbContextFactory<GameDbContext>
    {
        public GameDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<GameDbContext>();
            optionsBuilder.UseSqlite("Data Source=C:\\Users\\osho\\source\\repos\\SteamDiscountFinder\\SteamDiscountFinder.Data\\games.db"); // Ensure it's pointing to 'games.db'

            return new GameDbContext(optionsBuilder.Options);
        }
    }
}
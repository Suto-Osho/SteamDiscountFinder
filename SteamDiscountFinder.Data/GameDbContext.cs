using Microsoft.EntityFrameworkCore;
using SteamDiscountFinder.Data.Entities;

namespace SteamDiscountFinder.Data
{
    /// <summary>
    /// Entity Framework context for managing Game entities.
    /// </summary>
    public class GameDbContext : DbContext
    {
        public DbSet<Game> Games { get; set; }

        public GameDbContext(DbContextOptions<GameDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Additional configurations can go here
        }
    }
}
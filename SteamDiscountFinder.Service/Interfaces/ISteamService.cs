using SteamDiscountFinder.Data.Entities;

namespace SteamDiscountFinder.Service.Interfaces
{
    /// <summary>
    /// Defines methods for retrieving discounted games.
    /// </summary>
    public interface ISteamService
    {
        Task<List<Game>> GetDiscountedGamesAsync();
    }
}
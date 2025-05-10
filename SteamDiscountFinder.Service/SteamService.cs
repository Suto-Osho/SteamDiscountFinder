using System.Net.Http;
using System.Threading.Tasks;
using SteamDiscountFinder.Service.Interfaces;
using SteamDiscountFinder.Data.Entities;
using Newtonsoft.Json;

namespace SteamDiscountFinder.Service
{
    /// <summary>
    /// Retrieves discounted game data (mocked or from API).
    /// </summary>
    public class SteamService : ISteamService
    {
        private readonly HttpClient _httpClient;

        public SteamService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Game>> GetDiscountedGamesAsync()
        {
            // Mock data for now; we'll integrate an API later
            await Task.Delay(500); // Simulate async work

            return new List<Game>
            {
                new Game
                {
                    Title = "Mock Game 1",
                    OriginalPrice = 49.99m,
                    DiscountedPrice = 19.99m,
                    Url = "https://store.steampowered.com/app/123456",
                    RetrievedAt = DateTime.Now
                },
                new Game
                {
                    Title = "Mock Game 2",
                    OriginalPrice = 59.99m,
                    DiscountedPrice = 29.99m,
                    Url = "https://store.steampowered.com/app/789012",
                    RetrievedAt = DateTime.Now
                }
            };
        }
    }
}
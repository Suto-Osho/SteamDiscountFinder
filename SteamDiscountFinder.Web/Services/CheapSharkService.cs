using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SteamDiscountFinder.Web.Services
{
    public class CheapSharkService
    {
        private readonly HttpClient _httpClient;

        public CheapSharkService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<GameDeal>> GetSteamDealsAsync(
    string? title = null,
    string? sort = null,
    int? minPrice = null,
    int? maxPrice = null,
    int page = 1,
    int pageSize = 20)
        {
            var query = new List<string>
    {
        "storeID=1",
        $"page={page}",             // <-- This MUST reflect the passed-in `page`
        $"pageSize={pageSize}"
    };

            if (!string.IsNullOrWhiteSpace(title))
                query.Add($"title={Uri.EscapeDataString(title)}");

            query.Add($"sortBy={(string.IsNullOrWhiteSpace(sort) ? "Deal Rating" : sort)}");

            if (minPrice.HasValue)
                query.Add($"lowerPrice={minPrice.Value}");

            if (maxPrice.HasValue)
                query.Add($"upperPrice={maxPrice.Value}");

            var url = $"https://www.cheapshark.com/api/1.0/deals?{string.Join("&", query)}";

            var response = await _httpClient.GetStringAsync(url);
            var deals = JsonConvert.DeserializeObject<List<GameDeal>>(response);

            return deals ?? new List<GameDeal>();
        }
    }

    public class GameDeal
    {
        public string title { get; set; }
        public string normalPrice { get; set; }
        public string salePrice { get; set; }
        public string steamAppID { get; set; }
        public string dealID { get; set; }
    }
}

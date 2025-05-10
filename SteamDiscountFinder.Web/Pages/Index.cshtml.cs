using Microsoft.AspNetCore.Mvc.RazorPages;
using SteamDiscountFinder.Web.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SteamDiscountFinder.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly CheapSharkService _cheapSharkService;

        public List<GameDeal> Deals { get; set; } = new();

        public IndexModel(CheapSharkService cheapSharkService)
        {
            _cheapSharkService = cheapSharkService;
        }
        public int CurrentPage { get; set; } = 1;

        public async Task OnGetAsync(string? search, string? sort, int? minPrice, int? maxPrice, int currentPage = 1)
        {
            CurrentPage = currentPage;
            Deals = await _cheapSharkService.GetSteamDealsAsync(search, sort, minPrice, maxPrice, CurrentPage, 20);
        }

    }
}
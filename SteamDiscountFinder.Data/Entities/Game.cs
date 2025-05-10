using System;

namespace SteamDiscountFinder.Data.Entities
{
    /// <summary>
    /// Represents a game with its pricing and discount details.
    /// </summary>
    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public string Url { get; set; } = string.Empty;
        public DateTime RetrievedAt { get; set; }
    }
}
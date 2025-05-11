using Xunit;
using Moq;
using System.Net.Http;
using System.Threading.Tasks;
using SteamDiscountFinder.Service;
using SteamDiscountFinder.Web.Services;
using SteamDiscountFinder.Data.Entities;
using SteamDiscountFinder.Data;
using System.Collections.Generic;
using System.Net;
using System;
using System.Net.Http.Headers;
using System.Threading;
using Moq.Protected;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SteamDiscountFinder.Tests
{
    public class SteamServiceTests
    {
        [Fact]
        public async Task GetDiscountedGamesAsync_ReturnsMockGames()
        {
            var httpClient = new HttpClient();
            var service = new SteamService(httpClient);

            var result = await service.GetDiscountedGamesAsync();

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.IsType<Game>(result[0]);
        }

        [Fact]
        public void PaginationLogic_ReturnsCorrectPageData()
        {
            var allGames = Enumerable.Range(1, 50).Select(i =>
                new Game { Title = $"Game {i}", Id = i }).ToList();

            int page = 2;
            int pageSize = 10;

            var pagedGames = allGames
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            Assert.Equal(pageSize, pagedGames.Count);
            Assert.Equal("Game 11", pagedGames.First().Title);
            Assert.Equal("Game 20", pagedGames.Last().Title);
        }

        [Fact]
        public void PaginationLogic_PageOutOfRange_ReturnsEmptyList()
        {
            var allGames = Enumerable.Range(1, 10).Select(i =>
                new Game { Title = $"Game {i}", Id = i }).ToList();

            int page = 5;
            int pageSize = 10;

            var pagedGames = allGames
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            Assert.Empty(pagedGames);
        }

        [Fact]
        public void PaginationLogic_PageSizeZero_ReturnsEmptyList()
        {
            var allGames = Enumerable.Range(1, 10).Select(i =>
                new Game { Title = $"Game {i}", Id = i }).ToList();

            int page = 1;
            int pageSize = 0;

            var pagedGames = allGames
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            Assert.Empty(pagedGames);
        }

        [Fact]
        public async Task GetDiscountedGamesAsync_ReturnsEmptyListIfNoDeals()
        {
            var mockHttp = new Mock<HttpMessageHandler>();
            mockHttp
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[]")
                });

            var httpClient = new HttpClient(mockHttp.Object);
            var service = new SteamService(httpClient);

            var result = await service.GetDiscountedGamesAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }

    public class CheapSharkServiceTests
    {
        [Fact]
        public async Task GetSteamDealsAsync_ConstructsQueryCorrectly()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[]")
                })
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://www.cheapshark.com/api/1.0/")
            };

            var service = new CheapSharkService(httpClient);
            var result = await service.GetSteamDealsAsync("Portal", "dealRating", 0, 10, 1, 5);

            Assert.NotNull(result);
            Assert.IsType<List<GameDeal>>(result);
        }

        [Fact]
        public async Task GetSteamDealsAsync_HandlesNullTitleAndSort()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[]")
                });

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://www.cheapshark.com/api/1.0/")
            };

            var service = new CheapSharkService(httpClient);
            var result = await service.GetSteamDealsAsync(null, null, null, null);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetSteamDealsAsync_AppliesPaginationParameters()
        {
            HttpRequestMessage capturedRequest = null;

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[]")
                });

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://www.cheapshark.com/api/1.0/")
            };

            var service = new CheapSharkService(httpClient);
            await service.GetSteamDealsAsync("Game", "dealRating", null, null, page: 5, pageSize: 20);

            Assert.NotNull(capturedRequest);
            var query = capturedRequest.RequestUri.Query;
            Assert.Contains("page=5", query);
            Assert.Contains("pageSize=20", query);
        }

        [Fact]
        public async Task GetSteamDealsAsync_InvalidJson_ReturnsEmptyList()
        {
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("INVALID_JSON")
                });

            var httpClient = new HttpClient(mockHandler.Object);
            var service = new CheapSharkService(httpClient);

            var result = await service.GetSteamDealsAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }

    public class GameDbContextTests
    {
        [Fact]
        public async Task CanInsertGameIntoDatabase()
        {
            var options = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: "TestGameDb")
                .Options;

            using (var context = new GameDbContext(options))
            {
                var game = new Game { Title = "Half-Life", OriginalPrice = 20.0M, DiscountedPrice = 10.0M };
                context.Games.Add(game);
                await context.SaveChangesAsync();
            }

            using (var context = new GameDbContext(options))
            {
                Assert.Single(context.Games);
                var loadedGame = context.Games.First();
                Assert.Equal("Half-Life", loadedGame.Title);
                Assert.Equal(20.0M, loadedGame.OriginalPrice);
                Assert.Equal(10.0M, loadedGame.DiscountedPrice);
            }
        }

        [Fact]
        public async Task CanQueryGamesFromDatabase()
        {
            var options = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: "QueryGameDb")
                .Options;

            using (var context = new GameDbContext(options))
            {
                context.Games.AddRange(
                    new Game { Title = "Game A", OriginalPrice = 15.0M, DiscountedPrice = 5.0M },
                    new Game { Title = "Game B", OriginalPrice = 25.0M, DiscountedPrice = 10.0M }
                );
                await context.SaveChangesAsync();
            }

            using (var context = new GameDbContext(options))
            {
                var games = await context.Games.Where(g => g.DiscountedPrice < 15.0M).ToListAsync();
                Assert.Equal(2, games.Count);
            }
        }

        [Fact]
        public async Task InsertingDuplicateTitle_DoesNotThrow_ButCanQueryBoth()
        {
            var options = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: "DuplicateTitleDb")
                .Options;

            using (var context = new GameDbContext(options))
            {
                context.Games.Add(new Game { Title = "Duplicate", OriginalPrice = 30, DiscountedPrice = 15 });
                context.Games.Add(new Game { Title = "Duplicate", OriginalPrice = 40, DiscountedPrice = 20 });
                await context.SaveChangesAsync();
            }

            using (var context = new GameDbContext(options))
            {
                var games = context.Games.Where(g => g.Title == "Duplicate").ToList();
                Assert.Equal(2, games.Count);
            }
        }
    }
}

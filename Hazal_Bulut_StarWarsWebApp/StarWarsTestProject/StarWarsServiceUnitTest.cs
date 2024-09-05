using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using StarWarsWebApp.Models;
using System.Net;

namespace StarWarsTestProject
{
    public class StarWarsServiceUnitTest
    {
        [Fact]
        public async Task GetAllStarshipsAsync_ReturnsAllStarships()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var starshipResponsePage1 = new StarshipResponse
            {
                Results = new List<Starship>
            {
                new Starship { Name = "Millennium Falcon", Films = new List<string> { "https://swapi.dev/api/films/1/" } }
            },
                Next = "https://swapi.dev/api/starships/?page=2"
            };

            var starshipResponsePage2 = new StarshipResponse
            {
                Results = new List<Starship>
            {
                new Starship { Name = "X-wing" }
            },
                Next = null
            };

            var filmResponse = new Film { Title = "A New Hope" };

            mockHttpMessageHandler.Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(starshipResponsePage1)),
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(starshipResponsePage2)),
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(filmResponse)),
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var starshipService = new StarshipService(httpClient);

            var result = await starshipService.GetAllStarshipsAsync();

            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal(2, result.Count);
            Xunit.Assert.Equal("Millennium Falcon", result[0].Name);
            Xunit.Assert.Equal("X-wing", result[1].Name);
            Xunit.Assert.Equal("A New Hope", result[0].FilmNames);
        }

        [Fact]
        public async Task GetStarshipsAsync_ValidUrl_ReturnsStarshipResponse()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var starshipResponse = new StarshipResponse
            {
                Results = new List<Starship> { new Starship { Name = "Millennium Falcon" } }
            };

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(starshipResponse)),
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var starshipService = new StarshipService(httpClient);

            var result = await starshipService.GetStarshipsAsync("https://swapi.dev/api/starships/");

            Xunit.Assert.NotNull(result);
            Xunit.Assert.Single(result.Results);
            Xunit.Assert.Equal("Millennium Falcon", result.Results[0].Name);
        }
    }
}
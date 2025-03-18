using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using CatDogBearMicroservice.Models;
using CatDogBearMicroservice.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

public class PictureServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly PictureDbContext _dbContext;
    private readonly Mock<ILogger<PictureService>> _loggerMock;
    private readonly PictureService _pictureService;

    public PictureServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);

        var options = new DbContextOptionsBuilder<PictureDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _dbContext = new PictureDbContext(options);

        _loggerMock = new Mock<ILogger<PictureService>>();
        _pictureService = new PictureService(_httpClient, _dbContext, _loggerMock.Object);
    }

    [Fact]
    public async Task FetchPictures_ShouldReturnUrls_WhenApiResponseIsValid()
    {
        // Arrange
        var animalType = "cat";
        var numberOfPictures = 1;
        var apiUrl = $"https://api.thecatapi.com/v1/images/search?limit={numberOfPictures}";
        var responseContent = new List<CatPicture>
        {
            new CatPicture { Id = "1", Url = "https://example.com/cat1.jpg", Width = 500, Height = 500 }
        };
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(responseContent)
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString() == apiUrl),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _pictureService.FetchPictures(animalType, numberOfPictures);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("https://example.com/cat1.jpg", result[0]);
    }
}
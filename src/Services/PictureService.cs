using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CatDogBearMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CatDogBearMicroservice.Services
{
    public class PictureService
    {
        private readonly HttpClient _httpClient;
        private readonly PictureDbContext _dbContext;
        private readonly ILogger<PictureService> _logger;

        public PictureService(HttpClient httpClient, PictureDbContext dbContext, ILogger<PictureService> logger)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
            _logger = logger;
        }

        public class ApiException : Exception
        {
            public ApiException(string message, Exception innerException)
                : base(message, innerException)
            {
            }
        }

        public async Task<List<string>> FetchPictures(string animalType, int number)
        {
            _logger.LogInformation($"Fetching {number} pictures for {animalType}");

            string apiUrl = GetApiUrl(animalType, number);
            string response = await FetchApiResponse(apiUrl);

            _logger.LogInformation($"API response: {response}");

            return ParseResponse(animalType, response);
        }

        private string GetApiUrl(string animalType, int number)
        {
            switch (animalType.ToLower())
            {
                case "cat":
                    return $"https://api.thecatapi.com/v1/images/search?limit={number}";
                case "dog":
                    return $"https://dog.ceo/api/breeds/image/random/{number}";
                case "bear":
                    return $"https://api.bearapi.com/v1/images/search?limit={number}";
                default:
                    throw new ArgumentException("Invalid animal type. Supported types are: cat, dog, bear.");
            }
        }

        private async Task<string> FetchApiResponse(string apiUrl)
        {
            try
            {
                return await _httpClient.GetStringAsync(apiUrl);
            }
            catch (HttpRequestException e)
            {
                throw new ApiException("Error fetching API response", e);
            }
        }

private List<string> ParseResponse(string animalType, string response)
{
    var urls = new List<string>();

    _logger.LogInformation($"Parsing response for {animalType}: {response}");

        var options = new System.Text.Json.JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    if (animalType.Equals("cat", StringComparison.OrdinalIgnoreCase))
    {
        var catPictures = System.Text.Json.JsonSerializer.Deserialize<List<CatPicture>>(response, options);
        _logger.LogInformation($"Deserialized cat pictures: {catPictures}");
        foreach (var picture in catPictures)
        {
            _logger.LogInformation($"Cat picture Id: {picture.Id}");
            _logger.LogInformation($"Cat picture URL: {picture.Url}");
            _logger.LogInformation($"Cat picture Width: {picture.Width}");
            _logger.LogInformation($"Cat picture Height: {picture.Height}");
            urls.Add(picture.Url);
        }
    }
    else if (animalType.Equals("dog", StringComparison.OrdinalIgnoreCase))
    {
        var dogPictures = JsonConvert.DeserializeObject<DogPictureResponse>(response);
        _logger.LogInformation($"Deserialized dog pictures: {dogPictures.Message}");
        urls.AddRange(dogPictures.Message);
    }
    else if (animalType.Equals("bear", StringComparison.OrdinalIgnoreCase))
    {
        var bearPictures = System.Text.Json.JsonSerializer.Deserialize<List<BearPicture>>(response);
        _logger.LogInformation($"Deserialized bear pictures: {bearPictures}");
        urls.AddRange(bearPictures.Select(p => p.Url));
    }

    if (urls.Count == 0)
    {
        throw new Exception("No URLs found in the API response");
    }

    _logger.LogInformation($"Parsed URLs: {string.Join(", ", urls)}");

    return urls;
}

        public async Task SavePicture(string url, string animalType)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("URL cannot be null or empty", nameof(url));
            }

            _logger.LogInformation($"Saving picture: {url} for {animalType}");

            var picture = new Picture
            {
                Url = url,
                AnimalType = animalType.ToLower(), // Ensure consistent casing
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Pictures.Add(picture);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Saved picture: {url} for {animalType}");
        }

        public async Task<Picture> GetLastPicture(string animalType)
        {
            _logger.LogInformation($"Retrieving last picture for {animalType}");

            var picture = await _dbContext.Pictures
                .Where(p => p.AnimalType.Equals(animalType, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync();

            if (picture != null)
            {
                _logger.LogInformation($"Retrieved last picture: {picture.Url} for {animalType}");
            }
            else
            {
                _logger.LogInformation($"No pictures found for {animalType}");
            }

            return picture;
        }
    }

    public class CatPicture
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class DogPictureResponse
    {
        public List<string> Message { get; set; }
    }

    public class BearPicture
    {
        public string Url { get; set; }
    }
}
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
            string apiUrl = GetApiUrl(animalType, number);
            string response = await FetchApiResponse(apiUrl);
            return ParseResponse(animalType, response);
        }

        /// <summary>
        /// Constructs the API URL based on the animal type and number of pictures to fetch.
        /// </summary>
        /// <param name="animalType"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private string GetApiUrl(string animalType, int number)
        {
            return animalType.ToLower() switch
            {
                "cat" => $"https://api.thecatapi.com/v1/images/search?limit={number}",
                "dog" => $"https://dog.ceo/api/breeds/image/random/{number}",
                "bear" => $"https://api.bearapi.com/v1/images/search?limit={number}",
                _ => throw new ArgumentException("Invalid animal type. Supported types are: cat, dog, bear.")
            };
        }

        /// <summary>
        /// Fetches the API response for the specified URL.
        /// </summary>
        /// <param name="apiUrl"></param>
        /// <returns></returns>
        /// <exception cref="ApiException"></exception>
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

        /// <summary>
        /// Parses the API response to extract the URLs of the pictures.
        /// </summary>
        /// <param name="animalType"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private List<string> ParseResponse(string animalType, string response)
        {
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return animalType.ToLower() switch
            {
                "cat" => System.Text.Json.JsonSerializer.Deserialize<List<CatPicture>>(response, options)?.Select(p => p.Url).ToList(),
                "dog" => JsonConvert.DeserializeObject<DogPictureResponse>(response)?.Message,
                "bear" => System.Text.Json.JsonSerializer.Deserialize<List<BearPicture>>(response, options)?.Select(p => p.Url).ToList(),
                _ => throw new Exception("No URLs found in the API response")
            } ?? throw new Exception("No URLs found in the API response");
        }

        /// <summary>
        /// Saves a picture to the database.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="animalType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task SavePicture(string url, string animalType)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("URL cannot be null or empty", nameof(url));
            }

            var picture = new Picture
            {
                Url = url,
                AnimalType = animalType.ToLower(), // Ensure consistent casing
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Pictures.Add(picture);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves the last saved picture for the specified animal type.
        /// </summary>
        /// <param name="animalType"></param>
        /// <returns></returns>
        public async Task<Picture> GetLastPicture(string animalType)
        {
            return await _dbContext.Pictures
                .Where(p => p.AnimalType.Equals(animalType, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync();
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
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CatDogBearMicroservice.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace CatDogBearMicroservice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PicturesController : ControllerBase
    {
        private readonly PictureService _pictureService;
        private readonly ILogger<PicturesController> _logger;

        public PicturesController(PictureService pictureService, ILogger<PicturesController> logger)
        {
            _pictureService = pictureService;
            _logger = logger;
        }

        /// <summary>
        /// Saves pictures of the specified animal type.
        /// </summary>
        /// <param name="animalType">The type of animal to fetch pictures for.</param>
        /// <param name="numberOfPictures">The number of pictures to fetch.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost("save")]
        public async Task<IActionResult> SavePicture([FromQuery] string animalType, [FromQuery] int numberOfPictures)
        {
            if (string.IsNullOrEmpty(animalType) || numberOfPictures <= 0)
            {
                return BadRequest("Invalid parameters.");
            }

            try
            {
                var result = await _pictureService.FetchPictures(animalType, numberOfPictures);
                if (result == null || result.Count == 0)
                {
                    return NotFound("No pictures found.");
                }

                var saveTasks = result.Select(picture => _pictureService.SavePicture(picture, animalType));
                await Task.WhenAll(saveTasks);

                return Ok("Pictures saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving pictures.");
                return StatusCode(500, "An error occurred while saving pictures.");
            }
        }

        /// <summary>
        /// Retrieves the last saved picture for the specified animal type.
        /// </summary>
        /// <param name="animalType">The type of animal to fetch the last picture for.</param>
        /// <returns>An IActionResult containing the last picture or an error message.</returns>
        [HttpGet("last")]
        public async Task<IActionResult> GetLastPicture([FromQuery] string animalType)
        {
            if (string.IsNullOrEmpty(animalType))
            {
                return BadRequest("Invalid parameters.");
            }

            try
            {
                var picture = await _pictureService.GetLastPicture(animalType);
                if (picture == null)
                {
                    return NotFound("No pictures found for the specified animal type.");
                }

                return Ok(picture);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the last picture.");
                return StatusCode(500, "An error occurred while retrieving the last picture.");
            }
        }
    }
}
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CatDogBearMicroservice.Controllers
{
    [ApiController]
    [Route("error")]
    public class ErrorController : ControllerBase
    {
        [HttpGet]
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (context != null)
            {
                // Log the error details here, e.g., using a logging framework like Serilog or NLog
                // Example: _logger.LogError(context.Error, "An unexpected error occurred.");
            }
            return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
        }
    }
}
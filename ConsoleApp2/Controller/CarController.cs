using ConsoleApp2.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConsoleApp2.Controller
{

    [ApiController]
    [Route("api/cars")]
    public class CarController : ControllerBase
    {
        private readonly CarService _carService;

        public CarController(CarService carService)
        {
            _carService = carService;
        }

        [HttpGet("search")]
        public IActionResult SearchCars(
            [FromQuery] string? brand,
            [FromQuery] int? minYear,
            [FromQuery] int? maxMileage,
            [FromQuery] string? fuelType,
            [FromQuery] int? minPrice,
            [FromQuery] int? maxPrice,
            [FromQuery] string? sortBy,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1) return BadRequest("Invalid pagination parameters.");
            if (minPrice > maxPrice) return BadRequest("Minimum price cannot be greater than maximum price.");

            var result = _carService.GetFilteredCars(brand, minYear, maxMileage, fuelType, minPrice, maxPrice, sortBy,
                page, pageSize);
            return this.Ok(result);
        }
    }
}

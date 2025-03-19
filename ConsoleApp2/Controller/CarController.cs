using ConsoleApp2.Model;
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
            this._carService = carService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchCars(
            [FromQuery] CarFilter filter)
        {
            if (filter.Page < 1 || filter.PageSize < 1)
                return this.BadRequest("Invalid pagination parameters.");
    
            if (filter.MinPrice > filter.MaxPrice)
                return this.BadRequest("Minimum price cannot be greater than maximum price.");

            var result = await this._carService.GetFilteredCarsAsync(filter);
            return this.Ok(result);
        }
    }
}

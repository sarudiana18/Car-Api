using ConsoleApp2.Helpers;
using ConsoleApp2.Model;

namespace ConsoleApp2.Services;

public class CarService
{ 
    private readonly List<Car> _cars;
       
    public CarService()
    {
       this._cars = CsvLoader.LoadCarsFromCsv("car_price_dataset.csv");
    }

    public async Task<CarSearchResult> GetFilteredCarsAsync(CarFilter filter)
   {
       var filteredCars = _cars.AsQueryable();

    if (!string.IsNullOrEmpty(filter.Brand))
        filteredCars = filteredCars.Where(c => c.Brand.Contains(filter.Brand, StringComparison.OrdinalIgnoreCase));

    if (filter.MinYear.HasValue)
        filteredCars = filteredCars.Where(c => c.Year >= filter.MinYear);

    if (filter.MaxMileage.HasValue)
        filteredCars = filteredCars.Where(c => c.Mileage <= filter.MaxMileage);

    if (!string.IsNullOrEmpty(filter.FuelType))
        filteredCars = filteredCars.Where(c => c.FuelType.Equals(filter.FuelType, StringComparison.OrdinalIgnoreCase));

    if (filter.MinPrice.HasValue)
        filteredCars = filteredCars.Where(c => c.Price >= filter.MinPrice);

    if (filter.MaxPrice.HasValue)
        filteredCars = filteredCars.Where(c => c.Price <= filter.MaxPrice);

    // Sorting
    filteredCars = filter.SortBy?.ToLower() switch
    {
        "price_asc" => filteredCars.OrderBy(c => c.Price),
        "price_desc" => filteredCars.OrderByDescending(c => c.Price),
        "year_asc" => filteredCars.OrderBy(c => c.Year),
        "year_desc" => filteredCars.OrderByDescending(c => c.Year),
        "mileage_asc" => filteredCars.OrderBy(c => c.Mileage),
        "mileage_desc" => filteredCars.OrderByDescending(c => c.Mileage),
        _ => filteredCars
    };

    // Pagination
    var totalCount = filteredCars.Count();
    var carsList = filteredCars.Skip((filter.Page - 1) * filter.PageSize)
                               .Take(filter.PageSize)
                               .ToList();

    // Aggregations
    var averagePrice = carsList.Any() ? carsList.Average(c => c.Price) : 0;
    var mostCommonFuelType = carsList.GroupBy(c => c.FuelType)
                                     .OrderByDescending(g => g.Count())
                                     .Select(g => g.Key)
                                     .FirstOrDefault();
    var newestCarYear = carsList.Any() ? carsList.Max(c => c.Year) : 0;

    return new CarSearchResult
    {
        Results = carsList,
        Aggregations = new CarAggregations
        {
            AveragePrice = averagePrice,
            MostCommonFuelType = mostCommonFuelType,
            NewestCarYear = newestCarYear
        },
        Pagination = new PaginationInfo
        {
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        }
    };
   }
}
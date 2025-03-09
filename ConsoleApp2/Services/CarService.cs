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

    public object GetFilteredCars(string? brand, int? minYear, int? maxMileage, string? fuelType,
        int? minPrice, int? maxPrice, string? sortBy, int page, int pageSize)
   {
       var query = this._cars.AsQueryable();

       if (!string.IsNullOrEmpty(brand))
           query = query.Where(c => c.Brand.Contains(brand, StringComparison.OrdinalIgnoreCase));
       if (minYear.HasValue)
           query = query.Where(c => c.Year >= minYear.Value);
       if (maxMileage.HasValue)
           query = query.Where(c => c.Mileage <= maxMileage.Value);
       if (!string.IsNullOrEmpty(fuelType))
           query = query.Where(c => c.FuelType.Equals(fuelType, StringComparison.OrdinalIgnoreCase));
       if (minPrice.HasValue)
           query = query.Where(c => c.Price >= minPrice.Value);
       if (maxPrice.HasValue)
           query = query.Where(c => c.Price <= maxPrice.Value);

       query = sortBy switch
       {
           "price_asc" => query.OrderBy(c => c.Price),
           "price_desc" => query.OrderByDescending(c => c.Price),
           "year_asc" => query.OrderBy(c => c.Year),
           "year_desc" => query.OrderByDescending(c => c.Year),
           "mileage_asc" => query.OrderBy(c => c.Mileage),
           "mileage_desc" => query.OrderByDescending(c => c.Mileage),
           _ => query
       };

       var totalCount = query.Count();
       var results = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

       var aggregations = new
       {
           averagePrice = results.Any() ? results.Average(c => c.Price) : 0,
           mostCommonFuelType = results.GroupBy(c => c.FuelType).OrderByDescending(g => g.Count())
               .Select(g => g.Key).FirstOrDefault(),
           newestCarYear = results.Any() ? results.Max(c => c.Year) : 0
       };

       return new
       {
           results,
           aggregations,
           pagination = new { totalCount, page, pageSize }
       };
   }
}
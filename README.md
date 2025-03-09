# Car-Api

## Overview
This is a REST API that provides filtered car listings with price analysis capabilities using the provided Car Price Dataset. The API is built using ASP.NET Core and provides various filtering and sorting features on car listings.

The dataset used is available at: [Car Price Dataset on Kaggle](https://www.kaggle.com/datasets/asinow/car-price-dataset).

## Objective
Implement a GET endpoint for `/api/cars/search` that allows users to filter, sort, and paginate a list of car listings. It also provides aggregated statistics about the filtered results.

### Core Requirements
#### Dynamic Filters:
- **Brand**: Partial match (case-insensitive).
- **Minimum Production Year**: Filter cars based on the year.
- **Maximum Mileage**: Filter cars based on mileage.
- **Fuel Type**: Exact match.
- **Price Range**: Min and max prices.

#### Advanced Features:
- **Sorting**: Sort results by price, year, or mileage in ascending or descending order.
- **Pagination**: Control page number and size.
- **Aggregated Statistics**:
  - Average price.
  - Most common fuel type.
  - Newest car year.

### Example Request
```http
GET /api/cars/search?brand=ford&minYear=2018&maxPrice=25000&sortBy=year_desc&page=1&pageSize=10
```

Expected Response
```http
{
  "results": [
    /* Filtered cars */
  ],
  "aggregations": {
    "averagePrice": 22150,
    "mostCommonFuelType": "Petrol",
    "newestCarYear": 2022
  },
  "pagination": {
    "totalCount": 85,
    "page": 1,
    "pageSize": 10
  }
}

```

Technical Approach
1. Data Loading
The Car Price dataset is available in a CSV format. To simplify the implementation and avoid external database dependencies, the dataset is loaded into memory at the application’s startup.

Approach:

On application startup, the CSV file is parsed and converted into a list of car objects.
The dataset is stored in-memory (in a static list or singleton service) so that it's available for querying and filtering in subsequent API calls.
The in-memory data model consists of simple properties such as Brand, Year, Mileage, FuelType, Price, etc., representing each car listing.
2. Filtering and Querying with LINQ
Approach:

LINQ (Language Integrated Query) is used to manipulate and query the dataset.
The API supports dynamic filtering, allowing users to filter the car listings based on multiple criteria like brand, minYear, maxMileage, fuelType, minPrice, maxPrice, etc.
Each filter is applied conditionally — only applying filters that are specified by the user in the query string.
Example of a dynamic filter in LINQ:

```
var filteredCars = cars
    .Where(car => string.IsNullOrEmpty(brand) || car.Brand.Contains(brand, StringComparison.OrdinalIgnoreCase))
    .Where(car => minYear == null || car.Year >= minYear)
    .Where(car => maxMileage == null || car.Mileage <= maxMileage)
    .Where(car => string.IsNullOrEmpty(fuelType) || car.FuelType == fuelType)
    .Where(car => minPrice == null || car.Price >= minPrice)
    .Where(car => maxPrice == null || car.Price <= maxPrice);
```

3. Sorting Results
The sorting functionality allows users to sort the results by fields like price, year, or mileage in both ascending and descending orders.

Approach:

Based on the sortBy parameter in the query string (e.g., price_asc, year_desc), the API applies the appropriate sorting using LINQ’s OrderBy or OrderByDescending methods.
Example of sorting using LINQ:
```
switch (sortBy)
{
    case "price_asc":
        filteredCars = filteredCars.OrderBy(car => car.Price);
        break;
    case "price_desc":
        filteredCars = filteredCars.OrderByDescending(car => car.Price);
        break;
    case "year_asc":
        filteredCars = filteredCars.OrderBy(car => car.Year);
        break;
    case "year_desc":
        filteredCars = filteredCars.OrderByDescending(car => car.Year);
        break;
    case "mileage_asc":
        filteredCars = filteredCars.OrderBy(car => car.Mileage);
        break;
    case "mileage_desc":
        filteredCars = filteredCars.OrderByDescending(car => car.Mileage);
        break;
}
```
4. Pagination
Pagination is handled by returning only a subset of the data based on the specified page number and page size.

Approach:

The page and pageSize parameters allow users to specify which page they want to view and how many items per page.
The Skip and Take methods in LINQ are used to return the appropriate subset of data based on the requested page.
Example of pagination:
```
var pagedCars = filteredCars.Skip((page - 1) * pageSize).Take(pageSize).ToList();
```

5. Aggregated Statistics
The API provides aggregated statistics for the filtered results, including:

Average price of the cars in the filtered list.
Most common fuel type among the filtered cars.
The newest car year in the filtered list.
Approach:

Aggregations are calculated using LINQ methods like Average, GroupBy, and Max.
Example for calculating aggregated statistics:
```
var averagePrice = filteredCars.Average(car => car.Price);
var mostCommonFuelType = filteredCars
    .GroupBy(car => car.FuelType)
    .OrderByDescending(group => group.Count())
    .FirstOrDefault()?.Key;
var newestCarYear = filteredCars.Max(car => car.Year);
```

6. Error Handling and Input Validation
The API performs basic input validation to ensure the provided parameters are valid. Invalid inputs, such as negative prices or unreasonable year values, result in a 400 Bad Request response.

Approach:

Each query parameter is validated before processing. For example, if minPrice is greater than maxPrice, the API returns an error.
The application also handles missing parameters by applying default values (e.g., using null for optional filters).

namespace ConsoleApp2.Model;

public class CarSearchResult
{
    public List<Car> Results { get; set; }
    public CarAggregations Aggregations { get; set; }
    public PaginationInfo Pagination { get; set; }
}

public class CarAggregations
{
    public double AveragePrice { get; set; }
    public string MostCommonFuelType { get; set; }
    public int NewestCarYear { get; set; }
}

public class PaginationInfo
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
namespace ConsoleApp2.Model;

public class CarFilter
{
    public string? Brand { get; set; }
    public int? MinYear { get; set; }
    public int? MaxMileage { get; set; }
    public string? FuelType { get; set; }
    public int? MinPrice { get; set; }
    public int? MaxPrice { get; set; }
    public string? SortBy { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
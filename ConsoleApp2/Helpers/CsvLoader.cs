using ConsoleApp2.Model;

namespace ConsoleApp2.Helpers
{

    public static class CsvLoader
    {
        public static List<Car> LoadCarsFromCsv(string filePath)
        {
            return File.ReadLines(filePath)
                .Skip(1)
                .Select(line => line.Split(','))
                .Select(columns => new Car
                {
                    Brand = columns[0],
                    Model = columns[1],
                    Year = int.Parse(columns[2]),
                    Engine_Size = decimal.Parse(columns[3]),
                    FuelType = columns[4],
                    Transmission = columns[5],
                    Mileage = int.Parse(columns[6]),
                    Doors = int.Parse(columns[7]),
                    Owner_Count = int.Parse(columns[8]),
                    Price = int.Parse(columns[9])
                })
                .ToList();
        }
    }
}
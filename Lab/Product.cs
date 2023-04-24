using System.Text.Json.Serialization;

namespace Lab;

public class Product
{
    [JsonPropertyName("ProductID")] public int ProductId { get; set; }
    public string? ProductNumber { get; set; }
    public string? Name { get; set; }
    public string? Color { get; set; }

    public string? WeightUnitMeasureCode { get; set; }
    public double Weight { get; set; }

    public string? SizeUnitMeasureCode { get; set; }
    public string? Size { get; set; }

    public int DaysToManufacture { get; set; }
    public double StandardCost { get; set; }
    public double ListPrice { get; set; }
    public double ListPriceCost => StandardCost * ListPrice;

    public DateTime ModifiedDate { get; set; }
    public DateTime SellStartDate { get; set; }

    public List<ProductHistory> Products { get; }

    public Product()
    {
        Products = new List<ProductHistory>();
    }
}

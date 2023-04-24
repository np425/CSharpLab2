using System.Text.Json.Serialization;

namespace Lab;

public class ProductHistory: History
{
    [JsonPropertyName("ProductID")] public int ProductId { get; set; }
    
    public DateTime StartDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public double ListPrice { get; set; }

    public Product? Product { get; set; }

    protected override void PrintExpirationDate()
    {
        Console.WriteLine(EndDate);
    }
}

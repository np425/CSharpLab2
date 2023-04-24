using System.Text.Json;

namespace Lab;

/// <summary>
/// Reads, Saves and Provides access to products and product histories from corresponding JSON files
/// </summary>
/// <remarks>
/// Assumes ProductID is unique across all Products
/// If there exists multiple products with the same ProductID, unexpected things would happen
/// </remarks>
/// <remarks>
/// Performance could be improved by using sorted collections such as SortedList, SortedDictionary, SortedSet
/// </remarks>
public class DatabaseManager : ICrudRepository<Product>, ICrudRepository<ProductHistory>
{
    private readonly string _directoryPath;
    private readonly string _productsFileName;
    private readonly string _historiesFileName;

    public DatabaseManager(string directoryPath, string productsFileName, string historiesFileName)
    {
        _directoryPath = directoryPath;
        _productsFileName = productsFileName;
        _historiesFileName = historiesFileName;

        Products = new List<Product>();
        ProductsPriceHistories = new List<ProductHistory>();
    }

    // IMPROVEMENT: Preferably use SortedSet, SortedDictionary or SortedList for faster insertion/update/deletion of data
    /// <remarks>
    /// Assumes ProductID is unique across all Products
    /// If there exists multiple products with the same ProductID, unexpected things will happen
    /// </remarks>
    public List<Product> Products { get; private set; }

    public List<ProductHistory> ProductsPriceHistories { get; private set; }

    public void LoadData()
    {
        var productsFilePath = Path.Combine(_directoryPath, _productsFileName);
        var jsonString = File.ReadAllText(productsFilePath);
        Products = JsonSerializer.Deserialize<List<Product>>(jsonString) ?? Products;

        var historiesFilePath = Path.Combine(_directoryPath, _historiesFileName);
        jsonString = File.ReadAllText(historiesFilePath);
        ProductsPriceHistories = JsonSerializer.Deserialize<List<ProductHistory>>(jsonString) ?? ProductsPriceHistories;

        MakeConnections();
    }

    public void SaveData()
    {
        var jsonString = JsonSerializer.Serialize(Products);
        var productsFilePath = Path.Combine(_directoryPath, _productsFileName);
        File.WriteAllText(productsFilePath, jsonString);

        jsonString = JsonSerializer.Serialize(ProductsPriceHistories);
        var historiesFilePath = Path.Combine(_directoryPath, _historiesFileName);
        File.WriteAllText(historiesFilePath, jsonString);
    }

    /// <summary>
    /// Connects all products with all product histories
    /// </summary>
    private void MakeConnections()
    {
        Products.Sort((a, b) => a.ProductId.CompareTo(b.ProductId));
        ProductsPriceHistories.Sort((a, b) => a.ProductId.CompareTo(b.ProductId));

        using var productsEnumerator = Products.GetEnumerator();
        productsEnumerator.MoveNext(); // Initially, enumerator is before the first element

        foreach (var history in ProductsPriceHistories)
        {
            // Ascends list of products till it finds a product with ID >= history ID
            while (productsEnumerator.Current.ProductId < history.ProductId)
            {
                if (!productsEnumerator.MoveNext())
                {
                    return;
                }
            }

            // If product ID is higher than history ID, then there's no product for history
            if (productsEnumerator.Current.ProductId > history.ProductId)
            {
                continue;
            }

            history.Product = productsEnumerator.Current;
            productsEnumerator.Current.Products.Add(history);
        }
    }

    public void InsertRecord(ProductHistory record)
    {
        ProductsPriceHistories.Add(record);
        MakeConnection(record);
    }

    public void UpdateRecord(ProductHistory record)
    {
        var product = record.Product;
        if (product?.ProductId == record.ProductId) return;

        product?.Products.Remove(record);
        MakeConnection(record);
    }

    public void DeleteRecord(ProductHistory record)
    {
        ProductsPriceHistories.Remove(record);
        RemoveConnection(record);
    }

    public void InsertRecord(Product record)
    {
        Products.Add(record);
        MakeConnection(record);
    }

    public void UpdateRecord(Product record)
    {
        var products = record.Products;
        if (!products.Any() || products[0].ProductId == record.ProductId) return;

        RemoveConnection(record);
        MakeConnection(record);
    }

    public void DeleteRecord(Product record)
    {
        Products.Remove(record);
        RemoveConnection(record);
    }

    private void MakeConnection(ProductHistory history)
    {
        var product = FindProductById(history.ProductId);
        if (product == null) return;

        product.Products.Add(history);
        history.Product = product;
    }

    private static void RemoveConnection(ProductHistory history)
    {
        var product = history.Product;
        product?.Products.Remove(history);
        history.Product = null;
    }

    private void MakeConnection(Product product)
    {
        var histories = FindHistoriesById(product.ProductId);
        foreach (var history in histories)
        {
            product.Products.Add(history);
            history.Product = product;
        }
    }

    private static void RemoveConnection(Product product)
    {
        var products = product.Products;
        foreach (var history in products)
        {
            history.Product = null;
        }

        products.Clear();
    }

    private Product? FindProductById(int productId)
    {
        return Products.Find(product => product.ProductId == productId);
    }

    private IEnumerable<ProductHistory> FindHistoriesById(int productId)
    {
        return ProductsPriceHistories.FindAll(history => history.ProductId == productId);
    }
}

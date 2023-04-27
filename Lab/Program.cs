// See https://aka.ms/new-console-template for more information

using Lab;

var databaseManager = new DatabaseManager(Environment.CurrentDirectory, "Products.json", "ProductsPriceHistory.json");
databaseManager.LoadData();

var histories = databaseManager.ProductsPriceHistories;
var products = databaseManager.Products;

#region Part I

// 1. Select
// A)
var query1A = (from history in histories select history).ToList();
var method1A = histories.Select(h => h).ToList();

// B)
var query1B = (from history in histories select new {history.ProductId, history.ListPrice}).ToList();
var method1B = histories.Select(h => new {h.ProductId, h.ListPrice}).ToList();

// 2. Where
// C)
var query2C = (from history in histories where history.ProductId == 739 select history).ToList();
var method2C = histories.Where(h => h.ProductId == 739).ToList();

// D)
var query2D = (from history in histories where history.EndDate != null select history).ToList();
var method2D = histories.Where(h => h.EndDate != null).ToList();

// 3. Order
// E) 
var query3E = (from product in products orderby product.Name select product).ToList();
var method3E = products.OrderBy(p => p.Name).ToList();

// F)
var query3F = (from product in products
        orderby product.StandardCost descending
        select new
        {
            ProductName = product.Name,
            product.ProductNumber,
            product.Color,
            product.StandardCost
        })
    .ToList();

var method3F = products.OrderByDescending(product => product.StandardCost)
    .Select(product =>
        new {ProductName = product.Name, product.ProductNumber, product.Color, product.StandardCost})
    .ToList();

#endregion

#region Part II

// 4. Grouping
// G)
var query4G = (from history in histories
    group history by history.ProductId
    into historyGroup
    select new
    {
        ProductID = historyGroup.Key,
        MaxListPrice = historyGroup.MaxBy(h => h.ListPrice),
        ModificationCount = historyGroup.Count()
    }).ToList();
var method4G = histories.GroupBy(history => history.ProductId)
    .Select(historyGroup => new
    {
        ProductID = historyGroup.Key,
        MaxListPrice = historyGroup.MaxBy(h => h.ListPrice),
        ModificationCount = historyGroup.Count()
    }).ToList();

// H
var query4H = (from product in products
    where product.Name?.StartsWith("BK-") ?? false
    group product by new {product.Color, product.SizeUnitMeasureCode}
    into productGroup
    select new
    {
        ColorWithSizeUnitMeasureCode = $"${productGroup.Key.Color} ({productGroup.Key.SizeUnitMeasureCode})",
        SummaryListPrice = productGroup.Sum(p => p.ListPrice),
        AverageListPrice = productGroup.Average(p => p.ListPrice),
        MaxListPrice = productGroup.MaxBy(p => p.ListPrice),
        MinListPrice = productGroup.MinBy(p => p.ListPrice)
    }).ToList();

var method4H = products.Where(product => product.Name?.StartsWith("BK-") ?? false)
    .GroupBy(product => new {product.Color, product.SizeUnitMeasureCode})
    .Select(productGroup => new
    {
        ColorWithSizeUnitMeasureCode = $"${productGroup.Key.Color} ({productGroup.Key.SizeUnitMeasureCode})",
        SummaryListPrice = productGroup.Sum(p => p.ListPrice),
        AverageListPrice = productGroup.Average(p => p.ListPrice),
        MaxListPrice = productGroup.MaxBy(p => p.ListPrice),
        MinListPrice = productGroup.MinBy(p => p.ListPrice)
    }).ToList();

// 5. Join
// I)
var query5I = (from product in products
    join history in histories on product.ProductId equals history.ProductId
    select product).ToList();

// J)
var query5J = (from product in products
    join historyMin in histories on product.ProductId equals historyMin.ProductId
    where historyMin == histories.MinBy(h => h.StartDate)
    join historyMax in histories on product.ProductId equals historyMax.ProductId 
    where historyMax == histories.MaxBy(h => h.StartDate)
    where Math.Abs(historyMin.ListPrice - historyMax.ListPrice) > 0.00001
    select new
    {
        product.ProductId,
        product.Name,
        FirstPrice = historyMin.ListPrice,
        LastPrice = historyMax.ListPrice
    }).ToList();

// 6. 
var query6 = (from history in histories 
    where history.StartDate?.Year is 2012 or 2013
    group history by history.ProductId into historyGroup
    join product in products on historyGroup.Key equals product.ProductId
    select new
    {
        product.ProductId,
        product.Name,
        AverageListPrice = historyGroup.Average(h => h.ListPrice)
    }).Skip(3).Take(5).ToList();

# endregion
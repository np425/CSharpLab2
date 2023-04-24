// See https://aka.ms/new-console-template for more information

using Lab;

var databaseManager = new DatabaseManager(Environment.CurrentDirectory, "Products.json", "ProductsPriceHistory.json");
databaseManager.LoadData();

// TODO: Test if database manager works

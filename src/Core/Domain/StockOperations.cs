namespace Core.Domain;

public static class StockOperations
{
    // Інваріант охоплює ДВІ сутності одразу: Product і Warehouse.
    // Жодна з них одна не володіє обома фактами, тому правило живе тут, а не в Product чи Warehouse.
    public static void RegisterArrival(Product product, Warehouse warehouse, int amount)
    {
        if (warehouse.IsClosed)
            throw new InvalidOperationException(
                $"Склад {warehouse.Name} закритий — прийняти {product.Sku} неможливо");

        product.RegisterArrival(amount);
    }
}
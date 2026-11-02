using Core.Domain;

Console.WriteLine("Сценарій 1: успіх");
Product product = Product.Create("P-001", "sku-001", "Цемент М400 25кг", "шт", 100);
Console.WriteLine(product);
product.RegisterArrival(50);
product.Issue(30);
Console.WriteLine(product);

Console.WriteLine();
Console.WriteLine("Сценарій 2: порушення інваріантів");

TryDo("видача більша за залишок", () => product.Issue(1000));
TryDo("порожній SKU", () => Product.Create("P-002", " ", "Пісок", "т", 10));
TryDo("від'ємний залишок", () => Product.Create("P-003", "SKU-003", "Цегла", "шт", -5));

static void TryDo(string title, Action action)
{
    try
    {
        action();
        Console.WriteLine($" {title}: виняток НЕ спрацював — інваріант відсутній!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($" {title}: {ex.GetType().Name} — {ex.Message}");
    }
}
var (products, errors) = ProductDomainImporter.LoadValidated("data/sample.csv");
Console.WriteLine($"Валідних товарів: {products.Count}, помилок: {errors.Count}");

Console.WriteLine();
Console.WriteLine(" Додаткове завдання ");

var (validProducts, importErrors) = ProductDomainImporter.LoadValidated("data/sample.csv");
Console.WriteLine($"Валідних товарів з файлу: {validProducts.Count}, помилок: {importErrors.Count}");

Warehouse mainWarehouse = Warehouse.Create("W-001", "Головний склад");
mainWarehouse.Close();
TryDo("прихід на закритий склад", () => StockOperations.RegisterArrival(product, mainWarehouse, 10));

Movement movement = Movement.Create("M-001", "SKU-001", 20);
movement.TransitionTo(MovementStatus.Confirmed);
Console.WriteLine($"Переміщення {movement.Id}: {movement.Status}");
TryDo("повторне підтвердження", () => movement.TransitionTo(MovementStatus.Confirmed));
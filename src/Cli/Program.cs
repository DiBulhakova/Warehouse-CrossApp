using Core;
using Core.Abstractions;
using Core.Domain;
using Core.Services;
using Core.Storage;

bool useFile = args.Contains("--file");
bool useCache = args.Contains("--cache");
string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");

ICatalogStore store = useFile
    ? new FileCatalogStore(dataPath)
    : new InMemoryCatalogStore(SampleData.Products());

if (useCache)
    store = new CachingCatalogStore(store, Console.WriteLine);

//ICatalogStore store = StoreFactory.Create(args);

var service = new CatalogService(store);

Console.WriteLine($"Сховище: {store.GetType().Name}");
Console.WriteLine();

Console.WriteLine(" Сценарій успіху ");
var created = service.Add("SKU-101", "Кабель UTP cat6", "м", 50);
Console.WriteLine($"Додано: {created}");
service.Receive(created.Id, 25);
service.Issue(created.Id, 10);
Console.WriteLine($"Знайдено за id: {service.Find(created.Id)}");
Console.WriteLine();
Console.WriteLine("Усі записи:");
foreach (var p in service.All())
    Console.WriteLine($"  {p.Id}  {p.Sku,-10} {p.Name,-22} {p.Quantity,5} {p.Unit}");

Console.WriteLine();
Console.WriteLine("Позиції із залишком менше 50:");
foreach (var item in service.Search(x => x.Quantity < 50))
    Console.WriteLine($"  {item.Sku}  {item.Name}  {item.Quantity} {item.Unit}");

if (useCache)
{
    Console.WriteLine();
    Console.WriteLine("Демонстрація кешу:");
    service.All();
    service.Add("SKU-102", "Кабель коаксіальний", "м", 20);
    service.All();
}

Console.WriteLine();
Console.WriteLine(" Сценарій відмови");
TryDo("прихід на неіснуючий id", () => service.Receive("NO-SUCH-ID", 5));
TryDo("дубль id", () => store.Add(Product.Create(created.Id, "SKU-DUP", "Дубль", "шт", 1)));
TryDo("видача більша за залишок", () => service.Issue(created.Id, 9999));

static void TryDo(string title, Action action)
{
    try
    {
        action();
        Console.WriteLine($"  {title}: виняток НЕ спрацював!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  {title}: {ex.GetType().Name} — {ex.Message}");
    }
}
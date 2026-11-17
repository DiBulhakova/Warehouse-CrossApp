using Core;
using Core.Abstractions;
using Core.Events;
using Core.Services;
using Core.Storage;

Console.OutputEncoding = System.Text.Encoding.UTF8;

bool useFile = args.Contains("--file");
bool useCache = args.Contains("--cache");
string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");

ICatalogStore store = useFile
    ? new FileCatalogStore(dataPath)
    : new InMemoryCatalogStore(SampleData.Products());

if (useCache)
    store = new CachingCatalogStore(store, Console.WriteLine);

var service = new CatalogService(store);
Console.WriteLine($"Сховище: {store.GetType().Name}");
Console.WriteLine();

// ЛОГ-ФАЙЛ: ШЛЯХ ЧЕРЕЗ Path.Combine, КАТАЛОГ СТВОРЮЄ КОД
var logPath = Path.Combine(AppContext.BaseDirectory, "logs", "app.log");
Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

// ПІДПИСНИК 1: ЛОКАЛЬНИЙ МЕТОД (МОЖНА ВІДПИСАТИСЯ)
void OnCatalogChanged(object? sender, CatalogChangedEventArgs e)
{
    var line = $"{e.At:HH:mm:ss} {e.Kind,-8} {e.Id} {e.Delta,4:+0;-0} → залишок {e.QuantityAfter}";
    Console.WriteLine(line);
    try
    {
        File.AppendAllText(logPath, line + Environment.NewLine);
    }
    catch (IOException ex)
    {
        Console.WriteLine($"  не вдалося записати лог: {ex.Message}");
    }
}

service.Changed += OnCatalogChanged;

// ПІДПИСНИК 2: ЛЯМБДА ІЗ ЗАМИКАННЯМ НАД changes
int changes = 0;
service.Changed += (_, _) => changes++;

Console.WriteLine(" Сценарій успіху ");
var created = service.Add("SKU-101", "Кабель UTP cat6", "м", 50);
service.Receive(created.Id, 25);
service.Issue(created.Id, 10);
Console.WriteLine($"Подій отримано: {changes}");
Console.WriteLine();

Console.WriteLine(" Сценарій відмови (підписка активна) ");
TryDo("прихід з від'ємною кількістю", () => service.Receive(created.Id, -5));
TryDo("прихід на неіснуючий id", () => service.Receive("NO-SUCH-ID", 5));
TryDo("видача більша за залишок", () => service.Issue(created.Id, 9999));
Console.WriteLine($"Подій отримано: {changes} (не змінилось)");
Console.WriteLine();

Console.WriteLine(" Після відписки OnCatalogChanged ");
service.Changed -= OnCatalogChanged;
service.Receive(created.Id, 5);   // тиха операція
Console.WriteLine($"Залишок після тихої операції: {service.Find(created.Id)!.Quantity}");
Console.WriteLine($"Подій отримано: {changes}");   // +1 лише від лямбди
Console.WriteLine();

Console.WriteLine(" FindAll(p => p.Quantity < 50) ");
var low = service.FindAll(p => p.Quantity < 50);
if (low.Count == 0) Console.WriteLine("  нічого не знайдено");
foreach (var p in low)
    Console.WriteLine($"  {p.Id}  {p.Sku,-10} {p.Name,-22} {p.Quantity,5} {p.Unit}");

Console.WriteLine();
Console.WriteLine($"Лог: {logPath}");

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
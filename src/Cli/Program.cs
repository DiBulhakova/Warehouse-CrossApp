using Core.Dto;
using Core.Import;

string path = args.Length > 0 ? args[0] : Path.Combine("data", "sample.csv");

if (!File.Exists(path))
{
    Console.WriteLine($"Файл не знайдено: {Path.GetFullPath(path)}");
    return 1;
}

string extension = Path.GetExtension(path).ToLowerInvariant();

ImportResult<ProductDto> result = extension switch
{
    ".csv" => ProductCsvImporter.Load(path),
    ".json" => ProductJsonImporter.Load(path),
    _ => new ImportResult<ProductDto>([], [$"непідтримуване розширення файлу: {extension}"])
};

Console.WriteLine($"Завантажено записів: {result.Items.Count}");
foreach (ProductDto p in result.Items.Take(5))
    Console.WriteLine($" {p.Id,-6} {p.Sku,-10} {p.Name,-26} {p.Quantity,5} {p.Unit}");

if (result.Errors.Count > 0)
{
    Console.WriteLine($"Пропущено рядків: {result.Errors.Count}");
    foreach (string e in result.Errors)
        Console.WriteLine($" ! {e}");
}

int total = result.Items.Count + result.Errors.Count;
double errorRate = total > 0 ? result.Errors.Count * 100.0 / total : 0;
Console.WriteLine($"Статистика: усього {total}, прийнято {result.Items.Count}, пропущено {result.Errors.Count}, помилок {errorRate:F1}%");

if (path.Contains("mixed"))
{
    var mixed = MixedImporter.Load(path);
    Console.WriteLine($"\nЗмішаний імпорт: товарів {mixed.Products.Count}, складів {mixed.Warehouses.Count}, помилок {mixed.Errors.Count}");
}
return 0;